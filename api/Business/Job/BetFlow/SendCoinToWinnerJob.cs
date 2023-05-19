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
			.WithCronSchedule("0 /2 * * * ?")
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

			join _userbet in this.dbContext.sportUserBets on _tx.sport_user_bet_id equals _userbet.id
			join _coin in this.dbContext.currencies on _userbet.bet_currency_id equals _coin.id

			where SportWinnerBetRewardTxModelConst.SendRewardTxStatusList.Contains(_tx.tx_status)

			orderby _userbet.bet_currency_amount ascending

			select new {
				currency_code = _coin.code,
				currency_network = _coin.network,
				currency_decimals = _coin.decimals,

				bet_currency_amount = _userbet.bet_currency_amount,
				bet_odd_value = _userbet.bet_odd_value,

				rewardTx = _tx,
			}
		;
		var winnerRewards = await query.Take(100).ToArrayAsync();
		if (winnerRewards.Length == 0) {
			return;
		}

		// We will pay total tx-fee, and just take 0.1 ADA as tx-fee from winners.
		var fee_in_ada = 0.1m;
		var transactions = new List<CardanoNode_TxRawAssetsRequestBody.TransactionItem>();
		var senderAddrs = new HashSet<string>();

		//todo should choose lower reward for higher tx status? maybe need remove some high-reward item?

		foreach (var winnerReward in winnerRewards) {
			var sender_addr = winnerReward.rewardTx.sender_address;
			var coinSendToWinner = winnerReward.bet_currency_amount * winnerReward.bet_odd_value;

			senderAddrs.Add(sender_addr);

			var sendAssets = new List<CardanoNode_AssetInfo>();
			if (winnerReward.currency_network != MstCurrencyModelConst.Network.Cardano) {
				return;
			}

			// Send token and 1.4 ADA if need
			sendAssets.Add(new() {
				asset_id = winnerReward.currency_code,
				quantity = $"{(coinSendToWinner * DkMaths.Pow(10, winnerReward.currency_decimals)):0}",
			});
			if (winnerReward.currency_code != MstCurrencyModelConst.CODE_ADA) {
				sendAssets.Add(new() {
					asset_id = MstCurrencyModelConst.CODE_ADA,
					quantity = $"{AppConst.MIN_LOVELACE_TO_SEND}",
				});
			}

			transactions.Add(new() {
				sender_address = sender_addr,
				receiver_address = winnerReward.rewardTx.receiver_address,
				assets = sendAssets.ToArray(),
			});
		}

		var cnodeRequest = new CardanoNode_TxRawAssetsRequestBody {
			fee_payer_addresses = senderAddrs.ToArray(),
			transactions = transactions.ToArray()
		};
		var cnodeResponse = await this.cardanoNodeRepo.TxRawAssetsAsync(cnodeRequest);

		var tx_status = cnodeResponse.succeed ? SportWinnerBetRewardTxModelConst.TxStatus.SubmitSucceed : SportWinnerBetRewardTxModelConst.TxStatus.SubmitFailed;
		var tx_id = cnodeResponse.data?.tx_id;
		var tx_message = cnodeResponse.message.TruncateForShortLengthDk();

		// Update tx status
		foreach (var item in winnerRewards) {
			var rewardTx = item.rewardTx;

			rewardTx.fee_in_ada = fee_in_ada;
			rewardTx.tx_status = tx_status;
			rewardTx.tx_id = tx_id;
			rewardTx.tx_result_message = tx_message;
		}

		await this.dbContext.SaveChangesAsync();
	}
}
