namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class SendPredictionRewardJob : BaseJob<SendPredictionRewardJob> {
	private const string JOB_NAME = nameof(SendPredictionRewardJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<SendPredictionRewardJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 /2 * * * ?") // Run every 2 minutes
			.WithDescription(JOB_NAME)
		);
	}

	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly SystemDao systemWalletDao;

	private const int TOP_RANK_PLAYER_COUNT = 3;
	private static readonly int[] REWARD_ADA_AMOUNTS = new int[TOP_RANK_PLAYER_COUNT] {
		50, 30, 20
	};

	public SendPredictionRewardJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<SendPredictionRewardJob> logger,
		CardanoNodeRepo cardanoNodeRepo,
		SystemDao systemWalletDao
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger) {
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.systemWalletDao = systemWalletDao;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		// Step 1/2. Calc prediction rank and reward
		var matches_notYetCalcRank = await this.dbContext.sportMatches
			.Where(m =>
				m.predictable == true &&
				m.prediction_rank_calculated == false &&
				m.status == SportMatchModelConst.TimeStatus.Ended
			)
			.ToArrayAsync()
		;

		foreach (var match in matches_notYetCalcRank) {
			match.prediction_rank_calculated = true;

			var topWinners = await this.dbContext.sportPredictUsers
				.Where(m => m.sport_match_id == match.id)
				.Where(m => m.predict_home_score == match.home_score && m.predict_away_score == match.away_score)
				.OrderBy(m => m.predicted_at)
				.ThenBy(m => m.id)
				.Take(TOP_RANK_PLAYER_COUNT)
				.ToArrayAsync()
			;
			if (topWinners.Length == 0) {
				continue;
			}

			for (var index = topWinners.Length - 1; index >= 0; --index) {
				var winner = topWinners[index];

				winner.prediction_rank = index + 1;
				winner.reward_coin_amount = REWARD_ADA_AMOUNTS[index];
				winner.reward_submit_tx_status = SportPredictUserModelConst.RewardSubmitTxStatus.RequestSubmitReward;
			}
		}

		if (matches_notYetCalcRank.Length > 0) {
			await this.dbContext.SaveChangesAsync();
		}

		// Step 2. Send reward to winners
		var queryWinners =
			from _predict in this.dbContext.sportPredictUsers
			join _coin in this.dbContext.currencies on _predict.reward_coin_id equals _coin.id

			where _predict.reward_submit_tx_status == SportPredictUserModelConst.RewardSubmitTxStatus.RequestSubmitReward ||
				_predict.reward_submit_tx_status == SportPredictUserModelConst.RewardSubmitTxStatus.SubmitFailed

			select new {
				currency_code = _coin.code,
				currency_decimals = _coin.decimals,

				prediction = _predict,
			}
		;
		var rewardWinners = await queryWinners.ToArrayAsync();
		if (rewardWinners.Length == 0) {
			return;
		}

		var sysAddress = await this.systemWalletDao.GetSystemGameAddressAsync();

		var transactions = new List<CardanoNode_TxRawAssetsRequestBody.TransactionItem>();
		var sendAssets = new List<CardanoNode_AssetInfo>();

		foreach (var item in rewardWinners) {
			// Each winner will receive coin and 1.4 ADA if need
			sendAssets.Add(new() {
				asset_id = item.currency_code,
				quantity = $"{(item.prediction.reward_coin_amount * DkMaths.Pow(10, item.currency_decimals)):0}",
			});
			if (item.currency_code != MstCurrencyModelConst.CODE_ADA) {
				sendAssets.Add(new() {
					asset_id = MstCurrencyModelConst.CODE_ADA,
					quantity = $"{AppConst.MIN_LOVELACE_TO_SEND}",
				});
			}

			transactions.Add(new() {
				sender_address = sysAddress,
				receiver_address = item.prediction.reward_address,
				assets = sendAssets.ToArray()
			});
		}

		var cnodeRequest = new CardanoNode_TxRawAssetsRequestBody {
			fee_payer_addresses = new[] { sysAddress },
			transactions = transactions.ToArray(),
		};
		var cnodeResponse = await this.cardanoNodeRepo.TxRawAssetsAsync(cnodeRequest);

		var tx_id = cnodeResponse.data?.tx_id;
		var tx_message = cnodeResponse.message;
		var tx_status = cnodeResponse.succeed ? SportPredictUserModelConst.RewardSubmitTxStatus.SubmitSucceed : SportPredictUserModelConst.RewardSubmitTxStatus.SubmitFailed;

		foreach (var item in rewardWinners) {
			var prediction = item.prediction;

			prediction.reward_submit_tx_id = tx_id;
			prediction.reward_submit_tx_result_message = tx_message;
			prediction.reward_submit_tx_status = tx_status;
		}

		await this.dbContext.SaveChangesAsync();
	}
}
