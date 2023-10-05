namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;

[DisallowConcurrentExecution]
public class SubmitSwapToChainJob : BaseJob<SubmitSwapToChainJob> {
	private const string JOB_NAME = nameof(SubmitSwapToChainJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<SubmitSwapToChainJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 /2 * * * ?") // Run each minute
			.WithDescription(JOB_NAME)
		);
	}

	private readonly CardanoNodeRepo cardanoNodeRepo;

	public SubmitSwapToChainJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<SubmitSwapToChainJob> logger,
		CardanoNodeRepo cardanoNodeRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger) {
		this.cardanoNodeRepo = cardanoNodeRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		var query =
			from _coin_tx in this.dbContext.coinTxs

			where _coin_tx.action_type == CoinTxModelConst.ActionType.Swap
			where CoinTxModelConst.NeedSubmitToChainStatusList.Contains(_coin_tx.tx_status)

			orderby _coin_tx.updated_at ascending

			select new SelectPendingSwap {
				coinTx = _coin_tx,
			}
		;
		var results = await query.Take(1000).ToArrayAsync();
		if (results.Length == 0) {
			return;
		}

		// Group for each user (buyer, payer)
		var payer2swaps = new Dictionary<string, List<SelectPendingSwap>>();
		foreach (var item in results) {
			var payerAddress = item.coinTx.payer_address!;
			var payCoins = payer2swaps.GetValueOrDefault(payerAddress);
			if (payCoins is null) {
				payCoins = payer2swaps[payerAddress] = new();
			}
			payCoins.Add(item);
		}

		// Submit swap for each user (buyer, payer)
		foreach (var pendingSwaps in payer2swaps.Values) {
			var forwardCoinIds = pendingSwaps.Select(m => m.coinTx.forward_currency_id).ToList();
			var backwardCoinIds = pendingSwaps.Select(m => m.coinTx.backward_currency_id).ToList();
			var coinIds = forwardCoinIds.Concat(backwardCoinIds);
			var coins = await this.dbContext.currencies.AsNoTracking().Where(m => coinIds.Contains(m.id)).ToDictionaryAsync(m => m.id);

			// We will pay total tx-fee, and just take small (0.1) ADA as tx-fee from winners.
			var transactions = new List<CardanoNode_TxRawAssetsRequestBody.TransactionItem>();
			var feePayerAddresses = new HashSet<string>();

			// Build pendingSwaps cnode request for the user
			foreach (var pendingSwap in pendingSwaps) {
				var coinTx = pendingSwap.coinTx;

				var sellerInternalAddress = coinTx.seller_address;
				var buyerInternalAddress = coinTx.payer_address!;
				var receiverAddress = coinTx.receiver_address;

				feePayerAddresses.Add(coinTx.fee_payer_address);

				// Step 1/2. Forward: System (seller) -> User (buyer)
				var forwardCoin = coins[coinTx.forward_currency_id];
				var forwardAssets = new CardanoNode_AssetInfo[] {
					new() {
						asset_id = forwardCoin.code,
						quantity = $"{(coinTx.forward_currency_amount * DkMaths.Pow(10, forwardCoin.decimals)):0}",
					}
				};
				transactions.Add(new() {
					sender_address = sellerInternalAddress,
					receiver_address = receiverAddress,
					assets = forwardAssets,
				});
				// Buyer needs attach 1.4 ADA to receiver if send non-ADA coin.
				if (forwardCoin.code != MstCurrencyModelConst.CODE_ADA) {
					var attachAdaAssets = new CardanoNode_AssetInfo[] {
						new() {
							asset_id = MstCurrencyModelConst.CODE_ADA,
							quantity = $"{AppConst.MIN_LOVELACE_TO_SEND}",
						}
					};
					transactions.Add(new() {
						sender_address = buyerInternalAddress,
						receiver_address = receiverAddress,
						assets = attachAdaAssets,
					});
				}

				// Step 2/2. Backward: User (buyer) -> System (seller)
				var backwardCoin = coins[coinTx.backward_currency_id];
				var backwardAssets = new CardanoNode_AssetInfo[] {
					new() {
						asset_id = backwardCoin.code,
						quantity = $"{(coinTx.backward_currency_amount * DkMaths.Pow(10, backwardCoin.decimals)):0}",
					}
				};
				transactions.Add(new() {
					sender_address = buyerInternalAddress,
					receiver_address = sellerInternalAddress,
					assets = backwardAssets,
				});
				// Buyer needs attach 1.4 ADA to system if send non-ADA coin
				if (backwardCoin.code != MstCurrencyModelConst.CODE_ADA) {
					var attachAdaAssets = new CardanoNode_AssetInfo[] {
						new() {
							asset_id = MstCurrencyModelConst.CODE_ADA,
							quantity = $"{AppConst.MIN_LOVELACE_TO_SEND}",
						}
					};
					transactions.Add(new() {
						sender_address = buyerInternalAddress,
						receiver_address = sellerInternalAddress,
						assets = attachAdaAssets,
					});
				}
			}

			var cnodeRequest = new CardanoNode_TxRawAssetsRequestBody {
				fee_payer_addresses = feePayerAddresses.ToArray(),
				transactions = transactions.ToArray()
			};
			var cnodeResponse = await this.cardanoNodeRepo.TxRawAssetsAsync(cnodeRequest);

			var rewardTxStatus = cnodeResponse.succeed ? CoinTxModelConst.TxStatus.SubmitSucceed : CoinTxModelConst.TxStatus.SubmitFailed;
			var rewardTxHash = cnodeResponse.data?.tx_id;
			var rewardTxMessage = cnodeResponse.message;
			var feeInAda = (cnodeResponse.data?.fee / AppConst.ADA_COIN2TOKEN) ?? 0m;

			// Update reward
			foreach (var item in pendingSwaps) {
				var coinTx = item.coinTx;
				coinTx.tx_status = rewardTxStatus;
				coinTx.tx_hash = rewardTxHash;
				coinTx.tx_result_message = rewardTxMessage;
				coinTx.fee_in_ada = feeInAda;
			}
		}

		await this.dbContext.SaveChangesAsync();
	}

	private class SelectPendingSwap {
		public CoinTxModel coinTx;
	}
}
