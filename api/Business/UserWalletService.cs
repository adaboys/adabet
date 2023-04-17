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
	private readonly RedisComponent redisService;

	public UserWalletService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UserWalletService> logger,
		CardanoNodeRepo cardanoNodeRepo,
		UserDao userDao,
		ApiNodejsRepo apiNodejsRepo,
		RedisComponent redisService
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.userDao = userDao;
		this.apiNodejsRepo = apiNodejsRepo;
		this.redisService = redisService;
		this.cardanoNodeRepo = cardanoNodeRepo;
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
		var nonceCached = await this.redisService.SetStringAsync(
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
		var nonce = await this.redisService.GetStringAsync(RedisKey.ForLinkExternalWallet(external_wallet_address));
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
					.FirstOrDefaultAsync()
				;
				if (user is null) {
					return new ApiBadRequestResponse("Not found user");
				}

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
				await this.redisService.DeleteKeyAsync(RedisKey.ForLinkExternalWallet(external_wallet_address));

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
		var abeCoin = await this.dbContext.currencies.FirstAsync(m => m.name == MstCurrencyModelConst.NAME_ABE);
		foreach (var wallet in data_wallets) {
			var assetsResponse = await this.cardanoNodeRepo.GetMergedAssetsAsync(wallet.address);
			if (assetsResponse.failed) {
				return new ApiInternalServerErrorResponse("Could not get wallet assets");
			}

			wallet.ada_balance = CardanoHelper.CalcTotalAdaFromAssets(assetsResponse.data.assets);
			wallet.abe_balance = CardanoHelper.CalcTotalCoinFromAssets(abeCoin, assetsResponse.data.assets);
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
			return new ApiBadRequestResponse("No wallet");
		}

		// Soft delete wallet
		userWallet.deleted_at = DateTime.UtcNow;

		// Save
		await this.dbContext.SaveChangesAsync();

		return new ApiSuccessResponse($"Unlinked the wallet");
	}
}
