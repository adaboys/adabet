namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class SendRewardToWinnerJob : BaseJob<DecideUserBetResultJob> {
	private const string JOB_NAME = nameof(SendRewardToWinnerJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<SendRewardToWinnerJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 /2 * * * ?")
			.WithDescription(JOB_NAME)
		);
	}

	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly SystemDao systemDao;

	public SendRewardToWinnerJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<DecideUserBetResultJob> logger,
		MailComponent mailComponent,
		CardanoNodeRepo cardanoNodeRepo,
		SystemDao systemDao
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent) {
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.systemDao = systemDao;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		// Step 1/2. Request submit reward to chain.
		var ubets = await this.dbContext.sportUserBets
			// Submitted to chain
			.Where(m => SportUserBetModelConst.SucceedSubmissions.Contains(m.submit_tx_status))
			// Draw or won bet
			.Where(m => m.bet_result == SportUserBetModelConst.BetResult.Won || m.bet_result == SportUserBetModelConst.BetResult.Draw)
			// Not yet try submit reward to chain
			.Where(m => m.reward_tx_status == SportUserBetModelConst.RewardTxStatus.Nothing)
			.OrderByDescending(m => m.created_at)
			.Take(1000)
			.ToArrayAsync()
		;
		if (ubets.Length > 0) {
			var sysWalletAddress = await this.systemDao.GetSystemGameAddressAsync();
			foreach (var ubet in ubets) {
				ubet.reward_tx_status = SportUserBetModelConst.RewardTxStatus.RequestSubmitToChain;
				ubet.reward_sender_address ??= sysWalletAddress;
			}
			await this.dbContext.SaveChangesAsync();
		}

		// Step 2/2. Send reward to winners.
		var query =
			from _ubet in this.dbContext.sportUserBets

			join _coin in this.dbContext.currencies on _ubet.bet_currency_id equals _coin.id

			where SportUserBetModelConst.SendRewardTxStatusList.Contains(_ubet.reward_tx_status)

			orderby _ubet.bet_currency_amount ascending

			select new {
				currency_code = _coin.code,
				currency_network = _coin.network,
				currency_decimals = _coin.decimals,

				ubet = _ubet,
			}
		;
		var winnerRewards = await query.Take(100).ToArrayAsync();
		if (winnerRewards.Length == 0) {
			return;
		}

		// We will pay total tx-fee, and just take small (0.1) ADA as tx-fee from winners.
		var transactions = new List<CardanoNode_TxRawAssetsRequestBody.TransactionItem>();
		var senderAddresses = new HashSet<string>();

		foreach (var winnerItem in winnerRewards) {
			var ubet = winnerItem.ubet;

			// Only send if sender was set.
			// If sender is not set, it means user's bet is maybe not win yet.
			var senderAddress = ubet.reward_sender_address;
			if (senderAddress is null) {
				continue;
			}
			// Check network since we only handle for Cardano at this time.
			if (winnerItem.currency_network != MstCurrencyModelConst.Network.Cardano) {
				continue;
			}

			senderAddresses.Add(senderAddress);

			var sendAssets = new List<CardanoNode_AssetInfo>();
			var coinSendToWinner = ubet.bet_currency_amount * ubet.bet_odd_value;
			var isBetWithAda = winnerItem.currency_code == MstCurrencyModelConst.CODE_ADA;

			// Send token and 1.4 ADA if need
			sendAssets.Add(new() {
				asset_id = winnerItem.currency_code,
				quantity = $"{(coinSendToWinner * DkMaths.Pow(10, winnerItem.currency_decimals)):0}",
			});
			if (!isBetWithAda) {
				sendAssets.Add(new() {
					asset_id = MstCurrencyModelConst.CODE_ADA,
					quantity = $"{AppConst.MIN_LOVELACE_TO_SEND}",
				});
			}

			transactions.Add(new() {
				sender_address = senderAddress,
				receiver_address = ubet.reward_receiver_address,
				assets = sendAssets.ToArray(),
			});
		}

		if (transactions.Count == 0) {
			return;
		}

		var cnodeRequest = new CardanoNode_TxRawAssetsRequestBody {
			fee_payer_addresses = senderAddresses.ToArray(),
			transactions = transactions.ToArray()
		};
		var cnodeResponse = await this.cardanoNodeRepo.TxRawAssetsAsync(cnodeRequest);

		var rewardTxStatus = cnodeResponse.succeed ? SportUserBetModelConst.RewardTxStatus.SubmitSucceed : SportUserBetModelConst.RewardTxStatus.SubmitFailed;
		var rewardTxHash = cnodeResponse.data?.tx_id;
		var rewardTxMessage = cnodeResponse.message;

		// Update reward
		foreach (var item in winnerRewards) {
			var ubet = item.ubet;
			ubet.reward_tx_status = rewardTxStatus;
			ubet.reward_tx_hash = rewardTxHash;
			ubet.reward_tx_result_message = rewardTxMessage;
		}

		await this.dbContext.SaveChangesAsync();
	}
}
