namespace App;

using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tool.Compet.Core;

/// Raw query with interpolated: https://docs.microsoft.com/en-us/ef/core/querying/raw-sql
public class UserWalletService : BaseService {
	private readonly ILogger<UserWalletService> logger;
	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly UserDao userDao;
	private readonly ApiNodejsRepo apiNodejsRepo;
	private readonly RedisComponent redisComponent;
	private readonly MailComponent mailComponent;

	public UserWalletService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UserWalletService> logger,
		CardanoNodeRepo cardanoNodeRepo,
		UserDao userDao,
		ApiNodejsRepo apiNodejsRepo,
		RedisComponent redisComponent,
		MailComponent mailComponent
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.userDao = userDao;
		this.apiNodejsRepo = apiNodejsRepo;
		this.redisComponent = redisComponent;
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.mailComponent = mailComponent;
	}

	/// Just response signature (description + nonce)
	public async Task<ApiResponse> RequestLinkExternalWallet(Guid userId, string external_wallet_address) {
		// Must match with env
		if (!CardanoHelper.IsAddressMatchWithEnv(this.appSetting, external_wallet_address)) {
			return new ApiBadRequestResponse("Address must match with current env") { code = ErrCode.invalid_address };
		}
		// Validate address
		var validateRepsonse = await this.cardanoNodeRepo.ValidateAddressAsync(external_wallet_address);
		if (validateRepsonse.failed) {
			return new ApiFailureResponse(validateRepsonse.status, "Invalid address") { code = validateRepsonse.code };
		}

		// Cache nonce for the address for later re-build signature
		var nonce = CryptoHelper.GenerateNonce();
		var nonceCached = await this.redisComponent.SetStringAsync(
			RedisKey.ForLinkExternalWallet(external_wallet_address),
			nonce,
			TimeSpan.FromMinutes(30)
		);
		if (!nonceCached) {
			return new ApiInternalServerErrorResponse("Could not generate nonce");
		}

		return new RequestLinkExternalWalletResponse {
			data = new() {
				signature = CryptoHelper.MakeRequestSignatureInHex(nonce)
			}
		};
	}

	/// Perform link wallet
	public async Task<ApiResponse> LinkExternalWallet(Guid userId, string external_wallet_address, LinkExternalWalletRequestBody in_reqBody) {
		// Requires nonce
		var nonce = await this.redisComponent.GetStringAsync(RedisKey.ForLinkExternalWallet(external_wallet_address));
		if (nonce is null) {
			return new ApiBadRequestResponse("Login expired");
		}

		// Must valid user
		var user = await this.userDao.FindValidUserViaIdAsync(userId);
		if (user is null) {
			return new ApiBadRequestResponse("Invalid user");
		}

		// Validation:
		// - Verify ownership of the address
		var verifyAddressResponse = await this.apiNodejsRepo.VerifyAddressAsync(
			address: external_wallet_address,
			address_in_hex: in_reqBody.wallet_address_in_hex,
			request_signature: CryptoHelper.MakeRequestSignatureInHex(nonce),
			response_signature: in_reqBody.signed_signature,
			response_key: in_reqBody.signed_key
		);
		if (verifyAddressResponse.failed) {
			return new ApiBadRequestResponse("Unmatched ownership");
		}
		// - Validate address
		var validateResponse = await this.cardanoNodeRepo.ValidateAddressAsync(external_wallet_address);
		if (validateResponse.failed) {
			return new ApiBadRequestResponse("Invalid address");
		}

		// Lock the user to perform link wallet
		using (var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
			try {
				// Lock user
				user = await this.dbContext.users
					.FromSqlRaw($"SELECT * FROM [{DbConst.table_user}] WITH (UPDLOCK) WHERE id = {{0}}", user.id)
					.FirstAsync()
				;

				// The address must not exist in our system
				var userWallet = await this.dbContext.userWallets.FirstOrDefaultAsync(m => m.wallet_address == external_wallet_address);
				if (userWallet != null && userWallet.deleted_at == null) {
					return new ApiBadRequestResponse("Already linked");
				}

				// Add new wallet if not exist
				if (userWallet == null) {
					this.dbContext.userWallets.Attach(userWallet = new());
				}

				// Update wallet since it may existing (we don't delete wallet when unlink)
				userWallet.user_id = user.id;
				userWallet.wallet_name = in_reqBody.wallet_name;
				userWallet.wallet_address = external_wallet_address;
				userWallet.wallet_type = UserWalletModelConst.WalletType.External;
				userWallet.wallet_status = UserWalletModelConst.WalletStatus.Active;
				userWallet.deleted_at = null;

				await this.dbContext.SaveChangesAsync();
				txScope.Complete();

				// OK, delete nonce so request with the nonce will be invalid from now.
				await this.redisComponent.DeleteKeyAsync(RedisKey.ForLinkExternalWallet(external_wallet_address));

				return new LinkExternalWalletResponse {
					data = new() { }
				};
			}
			catch (Exception e) {
				this.logger.ErrorDk(this, "Could not link external wallet, data: {@data}", new Dictionary<string, object> {
					{ "userId", userId.ToString() },
					{ "external_wallet_address", external_wallet_address },
					{ "error", e.Message }
				});

				return new ApiInternalServerErrorResponse();
			}
		}
	}

	public async Task<ApiResponse> GetLinkedExternalWallets(Guid userId) {
		var linkedWallets = await this.dbContext.userWallets
			.Where(m =>
				m.user_id == userId &&
				m.wallet_type == UserWalletModelConst.WalletType.External &&
				m.deleted_at == null
			)
			.AsNoTracking()
			.ToArrayAsync()
		;

		if (linkedWallets.Length == 0) {
			return new ApiSuccessResponse("Empty");
		}

		var data_wallets = new List<GetLinkedExternalWalletsResponse.Wallet>(linkedWallets.Length);

		foreach (var wallet in linkedWallets) {
			data_wallets.Add(new() {
				name = wallet.wallet_name,
				address = wallet.wallet_address,
				status = (int)wallet.wallet_status
			});
		}

		// Query balance from Cardano
		var name2coin = await this.dbContext.currencies.AsNoTracking()
			.Where(m =>
				m.name == MstCurrencyModelConst.NAME_ABE ||
				m.name == MstCurrencyModelConst.NAME_GEM
			)
			.ToDictionaryAsync(m => m.name)
		;
		var abeCoin = name2coin[MstCurrencyModelConst.NAME_ABE];
		var gemCoin = name2coin[MstCurrencyModelConst.NAME_GEM];
		foreach (var item in data_wallets) {
			var assetsResponse = await this.cardanoNodeRepo.GetMergedAssetsAsync(item.address);
			if (assetsResponse.failed) {
				return new ApiInternalServerErrorResponse("Could not get wallet assets");
			}

			item.ada = CardanoHelper.CalcTotalAdaFromAssets(assetsResponse.data.assets);
			item.abe = CardanoHelper.CalcTotalCoinFromAssets(abeCoin, assetsResponse.data.assets);
			item.gem = CardanoHelper.CalcTotalCoinFromAssets(gemCoin, assetsResponse.data.assets);
		}

		return new GetLinkedExternalWalletsResponse {
			data = new() {
				wallets = data_wallets
			}
		};
	}

	public async Task<ApiResponse> UnlinkExternalWallet(Guid userId, string external_wallet_address) {
		var userWallet = await this.dbContext.userWallets.FirstOrDefaultAsync(m =>
			m.user_id == userId &&
			m.wallet_address == external_wallet_address &&
			m.wallet_type == UserWalletModelConst.WalletType.External &&
			m.deleted_at == null
		);
		if (userWallet is null) {
			return new ApiBadRequestResponse("No external wallet");
		}

		// Soft delete wallet
		userWallet.deleted_at = DateTime.UtcNow;

		// Save
		await this.dbContext.SaveChangesAsync();

		return new ApiSuccessResponse($"Unlinked the wallet");
	}

	public async Task<ActionResult<ApiResponse>> PrepareWithdraw(Guid sender_id, WithdrawCoinRequestBody in_reqBody) {
		// Get sender user
		var sender = await this.userDao.FindValidUserViaIdAsync(sender_id);
		if (sender is null) {
			return new ApiBadRequestResponse("Invalid user");
		}
		var senderWallet = await this.dbContext.userWallets.FirstOrDefaultAsync(m =>
			m.user_id == sender_id &&
			m.wallet_type == UserWalletModelConst.WalletType.Internal &&
			m.wallet_status == UserWalletModelConst.WalletStatus.Active
		);
		if (senderWallet is null) {
			return new ApiBadRequestResponse("Not found wallet");
		}

		// Check wallet balance and address existence
		var assetsResponse = await this.cardanoNodeRepo.GetMergedAssetsAsync(senderWallet.wallet_address);
		if (assetsResponse.failed) {
			return new ApiInternalServerErrorResponse("Could not check wallet balance");
		}
		var validateAddressResponse = await this.cardanoNodeRepo.ValidateAddressAsync(in_reqBody.receiver_address);
		if (validateAddressResponse.failed) {
			if (validateAddressResponse.code == ErrCode.invalid_address) {
				return new ApiBadRequestResponse { code = ErrCode.invalid_address };
			}
			return new ApiInternalServerErrorResponse("Could not touch receiver address");
		}

		var walletAssets = assetsResponse.data.assets;

		var name2coin = await this.dbContext.currencies.AsNoTracking()
			.Where(m =>
				m.name == MstCurrencyModelConst.NAME_ABE ||
				m.name == MstCurrencyModelConst.NAME_GEM
			)
			.ToDictionaryAsync(m => m.name)
		;
		var abeCoin = name2coin[MstCurrencyModelConst.NAME_ABE];
		var gemCoin = name2coin[MstCurrencyModelConst.NAME_GEM];

		var holdAdaAmount = CardanoHelper.CalcTotalAdaFromAssets(walletAssets);
		var holdAbeAmount = CardanoHelper.CalcTotalCoinFromAssets(abeCoin, walletAssets);
		var holdGemAmount = CardanoHelper.CalcTotalCoinFromAssets(gemCoin, walletAssets);

		// Not enoud ADA or ABE or GEM
		if (holdAdaAmount < in_reqBody.ada_amount || holdAbeAmount < in_reqBody.abe_amount || holdGemAmount < in_reqBody.gem_amount) {
			return new ApiBadRequestResponse { code = ErrCode.balance_not_enough };
		}

		// Cache in_reqBody at Redis server.
		// We use it as checksum to validate consistence of data (wallet address,...) at send-actual api.
		const int timeout = 30;
		in_reqBody.tmp_otp_code = CodeGenerator.GenerateOtpCode();
		var cacheKey = RedisKey.ForWithdrawCoin(sender_id.ToStringWithoutHyphen());
		var cached = await this.redisComponent.SetJsonAsync(cacheKey, in_reqBody, TimeSpan.FromMinutes(timeout));
		if (!cached) {
			return new ApiInternalServerErrorResponse("Could not gen otp");
		}

		// OK !
		// Send mail to user
		var sendMailResponse = await this.mailComponent.SendAsync(
			toEmail: sender.email,
			subject: "Withdraw Confirmation",
			body: await MailTemplate.ForWithdrawCoin_SendOtp(in_reqBody.tmp_otp_code, timeout)
		);
		if (sendMailResponse.failed) {
			return new ApiInternalServerErrorResponse("Could not send OTP");
		}

		return new ApiSuccessResponse();
	}

	public async Task<ApiResponse> PerformWithdrawActual(Guid sender_id, PerformWithdrawActualRequestBody requestBody) {
		// Get request body from cache
		var cacheKey = RedisKey.ForWithdrawCoin(sender_id.ToStringWithoutHyphen());
		var reqBody = await this.redisComponent.GetJsonAsync<WithdrawCoinRequestBody>(cacheKey);
		if (reqBody is null || reqBody.tmp_otp_code != requestBody.otp_code) {
			return new ApiBadRequestResponse("Invalid withdraw request");
		}

		// Get user and wallet
		var sender = await this.userDao.FindValidUserViaIdAsync(sender_id);
		var senderWallet = await this.dbContext.userWallets.FirstOrDefaultAsync(m =>
			m.user_id == sender_id &&
			m.wallet_type == UserWalletModelConst.WalletType.Internal &&
			m.wallet_status == UserWalletModelConst.WalletStatus.Active
		);
		if (sender is null || senderWallet is null) {
			return new ApiBadRequestResponse("Not found sender/wallet");
		}

		// [Preparation]
		// We don't sell so don't discount for them.
		var senderAddress = senderWallet.wallet_address;
		var receiverAddress = reqBody.receiver_address;
		var feePayerAddress = senderWallet.wallet_address;
		var discountFeeFromAssets = false;

		var name2coin = await this.dbContext.currencies.AsNoTracking()
			.Where(m =>
				m.name == MstCurrencyModelConst.NAME_ABE ||
				m.name == MstCurrencyModelConst.NAME_GEM
			)
			.ToDictionaryAsync(m => m.name)
		;
		var abeCoin = name2coin[MstCurrencyModelConst.NAME_ABE];
		var gemCoin = name2coin[MstCurrencyModelConst.NAME_GEM];

		// Must always send ADA (at least 1.4 ADA)
		var sendAssets = new List<CardanoNode_AssetInfo> {
			new() {
				asset_id = MstCurrencyModelConst.CODE_ADA,
				quantity = $"{(reqBody.ada_amount * AppConst.ADA_COIN2TOKEN):0}",
			}
		};
		if (reqBody.abe_amount > 0) {
			sendAssets.Add(new() {
				asset_id = abeCoin.code,
				quantity = $"{(reqBody.abe_amount * AppConst.ABE_COIN2TOKEN):0}",
			});
		}
		if (reqBody.gem_amount > 0) {
			sendAssets.Add(new() {
				asset_id = gemCoin.code,
				quantity = $"{(reqBody.gem_amount * AppConst.GEM_COIN2TOKEN):0}",
			});
		}

		// Note: Sender will pay the tx-fee.
		var cnodeRequest = new CardanoNode_SendAssetsRequestBody {
			force_send_all_assets = reqBody.send_all,
			sender_address = senderAddress,
			receiver_address = receiverAddress,
			fee_payer_address = senderAddress,
			discount_fee_from_assets = discountFeeFromAssets,
			assets = sendAssets.ToArray(),
		};

		// [Main 1/3] Save tx first
		var coinTx = new CoinTxModel {
			action_type = CoinTxModelConst.ActionType.SendAsWithdraw,

			// Sender
			seller_id = senderWallet.user_id,
			seller_address = senderAddress,

			// Receiver
			buyer_id = null, // no receiver id
			receiver_address = receiverAddress,

			// Amount to send
			forward_ada_amount = reqBody.ada_amount,
			forward_abe_amount = reqBody.abe_amount,
			forward_gem_amount = reqBody.gem_amount,

			fee_payer_id = senderWallet.user_id,
			fee_payer_address = feePayerAddress,
		};
		this.dbContext.coinTxs.Attach(coinTx);
		await this.dbContext.SaveChangesAsync();

		// [Main 2/3] Start tx
		var txFailed = true;
		string? txResultMessage = null;
		CardanoNode_SendAssetResponse? cnodeResponse = null;

		try {
			using (var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
				// Lock on sender
				sender = await this.dbContext.users
					.FromSqlRaw($"SELECT * FROM [{DbConst.table_user}] WITH (UPDLOCK) WHERE id = {{0}}", sender_id)
					.AsNoTracking()
					.FirstAsync()
				;

				// Send coin at Cardano
				cnodeResponse = await this.cardanoNodeRepo.SendAssetsAsync(cnodeRequest);
				txResultMessage = cnodeResponse.message;

				if (cnodeResponse.failed) {
					return new ApiResponse(cnodeResponse.status, this.appSetting.debug ? txResultMessage : "Could not send coin") {
						code = cnodeResponse.code
					};
				}

				// Send noti mail
				var sendMailResponse = await this.mailComponent.SendAsync(
					sender.email,
					"Withdraw successful",
					await MailTemplate.ForWithdrawCoin_SendActual(
						senderAddress,
						receiverAddress,
						reqBody.ada_amount,
						reqBody.abe_amount,
						reqBody.gem_amount,
						cnodeResponse.data.fee / AppConst.ADA_COIN2TOKEN
					)
				);

				// Post update our db
				coinTx.tx_status = CoinTxModelConst.TxStatus.SubmitSucceed;
				coinTx.tx_hash = cnodeResponse.data.tx_id;
				coinTx.tx_result_message = txResultMessage.TruncateForShortLengthDk();
				coinTx.fee_in_ada = cnodeResponse.data.fee / AppConst.ADA_COIN2TOKEN;
				coinTx.discounted_fee_in_ada = discountFeeFromAssets ? coinTx.fee_in_ada : 0;

				// Clear cache (otp, request body, ...)
				await this.redisComponent.DeleteKeyAsync(cacheKey);

				// Save and Commit
				await this.dbContext.SaveChangesAsync();
				txScope.Complete();
				txFailed = false;

				return new ApiSuccessResponse();
			}
		}
		catch (Exception e) {
			txResultMessage = e.Message;

			this.logger.CriticalDk(this, "Failed to send coin, data: {@data}", new Dictionary<string, object?> {
				{ "sender_id", sender_id.ToString() },
				{ "reqBody", reqBody },
				{ "error", e.Message },
				{ "cardanoRequest", cnodeRequest },
				{ "cardanoResponse", cnodeResponse },
			});

			return new ApiInternalServerErrorResponse();
		}
		// [Main 3/3] Update tx (we are at outside of tx)
		finally {
			if (txFailed || txResultMessage != null) {
				if (txFailed) {
					coinTx.tx_status = CoinTxModelConst.TxStatus.SubmitFailed;
				}
				coinTx.tx_result_message = txResultMessage.TruncateForShortLengthDk();

				await this.dbContext.SaveChangesAsync();
			}
		}
	}
}
