// namespace App;

// using System;
// using System.Text.Json.Serialization;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Options;
// using Quartz;
// using Tool.Compet.Core;
// using Tool.Compet.Json;

// /// This job is useful when user place bet too often (for eg,. more bets in 40 seconds)
// /// that causes failure submit to chain.
// /// So this job will try submit failed bet-tx to chain.
// [DisallowConcurrentExecution]
// public class SubmitUserBetToCardanoJob : BaseJob {
// 	private const string JOB_NAME = nameof(SubmitUserBetToCardanoJob);

// 	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
// 		quartzConfig.ScheduleJob<SubmitUserBetToCardanoJob>(trigger => trigger
// 			.WithIdentity(JOB_NAME)
// 			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
// 			.WithCronSchedule("0 /1 * * * ?") // Should fill from left -> right
// 			.WithDescription(JOB_NAME)
// 		);
// 	}

// 	private readonly ILogger<SubmitUserBetToCardanoJob> logger;
// 	private readonly CardanoNodeRepo cardanoNodeRepo;
// 	private readonly SystemWalletDao systemWalletDao;

// 	/// Each submit takes about 2 seconds.
// 	private const int MAX_PENDING_BETS = 20;

// 	public SubmitUserBetToCardanoJob(
// 		AppDbContext dbContext,
// 		IOptionsSnapshot<AppSetting> snapshot,
// 		ILogger<SubmitUserBetToCardanoJob> logger,
// 		CardanoNodeRepo cardanoNodeRepo,
// 		SystemWalletDao systemWalletDao
// 	) : base(dbContext, snapshot) {
// 		this.logger = logger;
// 		this.cardanoNodeRepo = cardanoNodeRepo;
// 		this.systemWalletDao = systemWalletDao;
// 	}

// 	/// Override
// 	public override async Task Run(IJobExecutionContext context) {
// 		var query =
// 			from _ubet in this.dbContext.sportUserBets

// 			join _match in this.dbContext.sportMatches on _ubet.sport_match_id equals _match.id
// 			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
// 			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
// 			join _wallet in this.dbContext.userWallets on _ubet.user_id equals _wallet.user_id
// 			join _coin in this.dbContext.currencies on _ubet.bet_currency_id equals _coin.id

// 			where _ubet.submit_tx_status == SportUserBetModelConst.TxStatus.SubmitFailed
// 			where _wallet.wallet_type == UserWalletModelConst.WalletType.Internal && _wallet.wallet_status == UserWalletModelConst.WalletStatus.Active

// 			// Choose older update since maybe we could not submit some bets (user does not enough balance,...).
// 			// With this sort method, after submitted a tx, the bet will be placed at rightmost when query.
// 			// At next time, we start submit from leftmost (newer bets).
// 			orderby _ubet.updated_at ascending

// 			select new {
// 				_wallet.wallet_address,

// 				home_team_name = _team1.name,
// 				away_team_name = _team2.name,

// 				match_start_at = _match.start_at,

// 				ubet = _ubet,

// 				bet_currency_name = _coin.name,
// 				bet_currency_code = _coin.code,
// 				bet_currency_network = _coin.network,
// 				bet_currency_decimals = _coin.decimals,
// 			}
// 		;
// 		var pendingBets = await query.Take(MAX_PENDING_BETS).ToArrayAsync();
// 		if (pendingBets.Length == 0) {
// 			return;
// 		}

// 		var sysWalletAddr = await this.systemWalletDao.GetSystem_MainForGame_AddressAsync();

// 		foreach (var item in pendingBets) {
// 			var ubet = item.ubet;

// 			// We use CIP-20 (datum label as 674) to attach user's bet as message
// 			// Ref: https://cips.cardano.org/cips/cip20
// 			var bet_memos = new string[] {
// 				$"t1: {item.home_team_name}".TruncateAsMetadataEntryDk(),
// 				$"t2: {item.away_team_name}".TruncateAsMetadataEntryDk(),
// 				$"start: {item.match_start_at.FormatDk()}".TruncateAsMetadataEntryDk(),
// 				$"market: {ubet.bet_market_name}".TruncateAsMetadataEntryDk(),
// 				$"odn: {ubet.bet_odd_name}".TruncateAsMetadataEntryDk(),
// 				$"odv: {ubet.bet_odd_value}".TruncateAsMetadataEntryDk(),
// 				$"bet: {ubet.bet_currency_amount} {item.bet_currency_name}".TruncateAsMetadataEntryDk(),
// 			};
// 			var betMetadata = new BetMetadata {
// 				cip_674 = new() {
// 					msg_list = bet_memos
// 				}
// 			};

// 			var sendAssets = new List<CardanoNode_AssetInfo>();

// 			if (item.bet_currency_network != MstCurrencyModelConst.Network.Cardano) {
// 				return;
// 			}

// 			// Send token and 1.4 ADA if need
// 			sendAssets.Add(new() {
// 				asset_id = item.bet_currency_code,
// 				quantity = $"{(ubet.bet_currency_amount * DkMaths.Pow(10, item.bet_currency_decimals)):0}"
// 			});
// 			if (item.bet_currency_code != MstCurrencyModelConst.CODE_ADA) {
// 				sendAssets.Add(new() {
// 					asset_id = MstCurrencyModelConst.CODE_ADA,
// 					quantity = $"{AppConst.MIN_LOVELACE_TO_SEND}"
// 				});
// 			}

// 			var cardanoRequest = new CardanoNode_SendAssetsRequestBody {
// 				sender_address = item.wallet_address,
// 				receiver_address = sysWalletAddr,
// 				fee_payer_address = item.wallet_address,
// 				discount_fee_from_assets = false,

// 				assets = sendAssets.ToArray(),

// 				metadata = betMetadata
// 			};

// 			var cardanoResponse = await this.cardanoNodeRepo.SendAssetsAsync(cardanoRequest);

// 			ubet.submit_tx_status = cardanoResponse.succeed ? SportUserBetModelConst.TxStatus.SubmitSucceed : SportUserBetModelConst.TxStatus.SubmitFailed;
// 			ubet.submit_tx_id = cardanoResponse.data?.tx_id;
// 			ubet.submit_tx_result_message = cardanoResponse.message.TruncateForShortLengthDk();
// 		}

// 		await this.dbContext.SaveChangesAsync();
// 	}
// }
