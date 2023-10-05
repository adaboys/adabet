namespace App;

using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tool.Compet.Core;
using Tool.Compet.EntityFrameworkCore;
using Tool.Compet.Json;

public class UserCoinTxService : BaseService {
	private readonly ILogger<UserCoinTxService> logger;
	private readonly BlockfrostRepo blockfrostRepo;
	private readonly SystemDao systemDao;

	public UserCoinTxService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UserCoinTxService> logger,
		BlockfrostRepo blockfrostRepo,
		SystemDao systemDao
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.blockfrostRepo = blockfrostRepo;
		this.systemDao = systemDao;
	}

	public async Task<ApiResponse> GetCoinTxHistory(
		Guid user_id,
		CoinTxModelConst.ActionType action,
		int? coin_id,
		int pagePos, int pageSize
	) {
		var query =
			from _tx in this.dbContext.coinTxs
			join _coin in this.dbContext.currencies on _tx.forward_currency_id equals _coin.id

			where _tx.action_type == action
			where _tx.seller_id == user_id || _tx.buyer_id == user_id
			where coin_id == null || (_tx.forward_currency_id == coin_id || _tx.backward_currency_id == coin_id)

			orderby _tx.created_at descending

			select new GetCoinTxHistoryResponse.History {
				created_at = _tx.created_at,
				updated_at = _tx.updated_at,

				sender_address = _tx.seller_address,
				send_coin = _tx.forward_currency_id,
				send_amount = _tx.forward_currency_amount,

				receiver_address = _tx.receiver_address,
				receive_coin = _tx.backward_currency_id,
				receive_amount = _tx.backward_currency_amount,
				attach_ada_amount = _tx.attach_ada_amount,

				status = (int)_tx.tx_status,
				tx_hash = _tx.tx_hash,

				fee_in_ada = _tx.fee_in_ada
			}
		;
		var pagedResult = await query.AsNoTracking().PaginateDk(pagePos, pageSize);
		var histories = pagedResult.items;

		return new GetCoinTxHistoryResponse {
			data = new() {
				page_pos = pagedResult.pagePos,
				page_count = pagedResult.pageCount,
				total_item_count = pagedResult.totalItemCount,
				histories = histories
			}
		};
	}

	public async Task<ApiResponse> GetAllTxHistories(
		Guid user_id,
		int? coin_id,
		int pagePos, int pageSize
	) {
		var coin = await this.dbContext.currencies.AsNoTracking().FirstAsync(m => m.id == coin_id);
		var userWalletAddress = await this.dbContext.userWallets
			.Where(m =>
				m.user_id == user_id &&
				m.wallet_type == UserWalletModelConst.WalletType.Internal &&
				m.wallet_status == UserWalletModelConst.WalletStatus.Active
			)
			.Select(m => m.wallet_address)
			.FirstAsync()
		;
		var txs = await this.blockfrostRepo.GetTransactionsAsync(userWalletAddress, pagePos, pageSize);
		if (txs is null) {
			return new ApiInternalServerErrorResponse("No data");
		}

		var hasNextPageResponse = await this.blockfrostRepo.GetTransactionsAsync(userWalletAddress, pagePos + 1, pageSize);
		var coinAsset = coin.code.Replace(".", string.Empty);
		var histories = new List<GetAllCoinTxHistoryResponse.History>();

		foreach (var tx in txs) {
			var utxo = await this.blockfrostRepo.GetTransactionUtxosAsync(tx.tx_hash);
			if (utxo != null) {
				var totalInAda = 0m;
				foreach (var input in utxo.inputs) {
					if (input.address == userWalletAddress) {
						foreach (var amount in input.amount) {
							if (amount.unit == coinAsset) {
								totalInAda += amount.quantity.ParseDecimalDk() / AppConst.ADA_COIN2TOKEN;
							}
						}
					}
				}

				var totalOutAda = 0m;
				foreach (var output in utxo.outputs) {
					if (output.address == userWalletAddress) {
						foreach (var amount in output.amount) {
							if (amount.unit == coinAsset) {
								totalOutAda += amount.quantity.ParseDecimalDk() / AppConst.ADA_COIN2TOKEN;
							}
						}
					}
				}

				histories.Add(new GetAllCoinTxHistoryResponse.History {
					receive_amount = totalOutAda - totalInAda,
					tx_hash = utxo.hash,
					created_at = DkDateTimes.ConvertUnixTimeSecondsToUtcDatetime(tx.block_time)
				});
			}
		}

		return new GetAllCoinTxHistoryResponse {
			data = new() {
				page_pos = pagePos,
				hasNextPage = hasNextPageResponse != null && hasNextPageResponse.Length > 0,
				histories = histories
			}
		};
	}

	public async Task<ApiResponse> CalcSwapCoinAmount(int src_coin, int dst_coin, decimal srcAmount) {
		var srcCoinWeight = await this.dbContext.currencies.Where(m => m.id == src_coin).Select(m => m.weight).FirstAsync();
		var dstCoinWeight = await this.dbContext.currencies.Where(m => m.id == dst_coin).Select(m => m.weight).FirstAsync();

		return new CalcSwapCoinAmountResponse {
			data = new() {
				amount = srcAmount * srcCoinWeight / dstCoinWeight
			}
		};
	}

	public async Task<ApiResponse> SwapCoin(Guid user_id, SwapCoinPayload payload) {
		if (payload.src_coin == payload.dst_coin) {
			return new ApiBadRequestResponse("Src and Dst coin must be different");
		}
		var srcCoin = await this.dbContext.currencies.AsNoTracking().FirstAsync(m => m.id == payload.src_coin);
		var dstCoin = await this.dbContext.currencies.AsNoTracking().FirstAsync(m => m.id == payload.dst_coin);

		var sysSwapAddress = await this.systemDao.GetSystemSwapAddressAsync();
		var userWallet = await this.dbContext.userWallets
			.Where(m =>
				m.user_id == user_id &&
				m.wallet_type == UserWalletModelConst.WalletType.Internal &&
				m.wallet_status == UserWalletModelConst.WalletStatus.Active &&
				m.deleted_at == null
			)
			.FirstOrDefaultAsync()
		;
		if (userWallet is null) {
			return new ApiBadRequestResponse("Not found user internal wallet");
		}

		var receiverAddress = userWallet.wallet_address;
		if (payload.receiver_wallet != null && payload.receiver_wallet.Length > 0) {
			receiverAddress = payload.receiver_wallet;
		}

		// After save to db, cronjob will schedule submit it to chain.
		var srcCoinAmount = payload.amount;
		this.dbContext.coinTxs.Attach(new() {
			action_type = CoinTxModelConst.ActionType.Swap,
			tx_status = CoinTxModelConst.TxStatus.RequestSubmitToChain,

			seller_id = null,
			seller_address = sysSwapAddress,

			buyer_id = user_id,
			payer_address = userWallet.wallet_address, // Pay with internal wallet
			receiver_address = receiverAddress, // Can receive at external wallet
			fee_payer_address = userWallet.wallet_address, // User need pay the fee

			// Conversion equation: src_coin * src_weight = dst_coin * dst_weight
			// Seller ---> dst_coin ---> Buyer
			forward_currency_id = payload.dst_coin,
			forward_currency_amount = srcCoinAmount * srcCoin.weight / dstCoin.weight,

			// Buyer ---> src_coin ---> Seller
			backward_currency_id = payload.src_coin,
			backward_currency_amount = srcCoinAmount
		});

		await this.dbContext.SaveChangesAsync();

		return new ApiSuccessResponse("Your swap order will be scheduled to submit to chain soon");
	}
}
