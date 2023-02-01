namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class SendCoinToWinnerJob : BaseJob {
	private const string JOB_NAME = nameof(SendCoinToWinnerJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<SendCoinToWinnerJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 /1 * * * ?") // Should fill from left -> right
			.WithDescription(JOB_NAME)
		);
	}

	private readonly ILogger<SendCoinToWinnerJob> logger;
	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly SystemWalletDao systemWalletDao;

	public SendCoinToWinnerJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<SendCoinToWinnerJob> logger,
		CardanoNodeRepo cardanoNodeRepo,
		SystemWalletDao systemWalletDao
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.systemWalletDao = systemWalletDao;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		var query =
			from _tx in this.dbContext.sportWinnerBetRewardTxs

			where (_tx.tx_status == SportWinnerBetRewardTxModelConst.TxStatus.RequestSubmitToChain || _tx.tx_status == SportWinnerBetRewardTxModelConst.TxStatus.SubmitFailed)

			orderby _tx.reward_ada_amount ascending

			select new {
				rewardTx = _tx,
			}
		;
		var totalRewardItems = await query.Take(100).ToArrayAsync();
		if (totalRewardItems.Length == 0) {
			return;
		}

		// We will pay total tx-fee, and just take 0.1 ADA as tx-fee from winners.
		var fee_in_ada = 0.1m;
		var transactions = new List<CardanoNode_TxRawAssetsRequestBody.TransactionItem>();
		var senderAddrs = new HashSet<string>();

		//todo should choose lower reward for higher tx status? maybe need remove some high-reward item?

		foreach (var item in totalRewardItems) {
			var sender_addr = item.rewardTx.sender_address;
			var adaSendToWinner = item.rewardTx.reward_ada_amount - fee_in_ada;

			senderAddrs.Add(sender_addr);

			transactions.Add(new() {
				sender_address = sender_addr,
				receiver_address = item.rewardTx.receiver_address,
				assets = new CardanoNode_AssetInfo[] {
					new() {
						asset_id = MstCardanoCoinModelConst.ASSET_ID_ADA,
						quantity = $"{(adaSendToWinner * AppConst.ADA_COIN2TOKEN):0}",
					}
				}
			});
		}
		var cardanoRequest = new CardanoNode_TxRawAssetsRequestBody {
			fee_payer_addresses = senderAddrs.ToArray(),
			transactions = transactions.ToArray()
		};
		var cardanoResponse = await this.cardanoNodeRepo.TxRawAssetsAsync(cardanoRequest);

		var tx_status = cardanoResponse.succeed ? SportWinnerBetRewardTxModelConst.TxStatus.SubmitSucceed : SportWinnerBetRewardTxModelConst.TxStatus.SubmitFailed;
		var tx_id = cardanoResponse.data?.tx_id;
		var tx_message = cardanoResponse.message.TruncateIfLongDk();

		// Update tx status
		foreach (var item in totalRewardItems) {
			item.rewardTx.fee_in_ada = fee_in_ada;
			item.rewardTx.tx_status = tx_status;
			item.rewardTx.tx_id = tx_id;
			item.rewardTx.tx_result_message = tx_message;
		}

		await this.dbContext.SaveChangesAsync();
	}
}
