namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class DecideUserBetResultJob : BaseJob {
	private const string JOB_NAME = nameof(DecideUserBetResultJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		//todo comment for now to avoid waste
		// quartzConfig.ScheduleJob<DecideUserBetResultJob>(trigger => trigger
		// 	.WithIdentity(JOB_NAME)
		// 	.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
		// 	.WithCronSchedule("0 /1 * * * ?") // Should fill from left -> right
		// 	.WithDescription(JOB_NAME)
		// );
	}

	private const int FulltimeSeconds = 90 * 60;

	private readonly ILogger<DecideUserBetResultJob> logger;
	private readonly SystemWalletDao systemWalletDao;

	public DecideUserBetResultJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<DecideUserBetResultJob> logger,
		SystemWalletDao systemWalletDao
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.systemWalletDao = systemWalletDao;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		var query =
			from _ubet in this.dbContext.sportUserBets
			join _match in this.dbContext.sportMatches on _ubet.sport_match_id equals _match.id

			where _ubet.bet_result == SportUserBetModelConst.BetResult.Nothing

			orderby _ubet.created_at descending

			select new {
				cur_play_time = _match.cur_play_time,
				home_score = _match.home_score,
				away_score = _match.away_score,

				ubet = _ubet,
			}
		;
		var matchItems = await query.Take(100).ToArrayAsync();

		var sysAdaWalletAddress = await this.systemWalletDao.GetSystem_MainForGame_AddressAsync();

		foreach (var matchItem in matchItems) {
			var ubet = matchItem.ubet;

			switch (ubet.bet_market_name) {
				case MarketConst.MainFullTime: {
					// Only check if the match end
					var matchEnded = matchItem.cur_play_time >= FulltimeSeconds;

					if (matchEnded) {
						var homeWin = (matchItem.home_score > matchItem.away_score);
						var draw = (matchItem.home_score == matchItem.away_score);
						var awayWin = (matchItem.home_score < matchItem.away_score);

						if (ubet.bet_odd_name == OddConst.HomeWin) {
							ubet.bet_result = homeWin ? SportUserBetModelConst.BetResult.Won : SportUserBetModelConst.BetResult.Losed;
						}
						else if (ubet.bet_odd_name == OddConst.Draw) {
							ubet.bet_result = draw ? SportUserBetModelConst.BetResult.Won : SportUserBetModelConst.BetResult.Losed;
						}
						else if (ubet.bet_odd_name == OddConst.AwayWin) {
							ubet.bet_result = awayWin ? SportUserBetModelConst.BetResult.Won : SportUserBetModelConst.BetResult.Losed;
						}
					}
					break;
				}
				case MarketConst.BothToScore: {
					var matchEnded = matchItem.cur_play_time >= FulltimeSeconds;

					if (matchEnded) {
						var bothToScore = (matchItem.home_score > 0 && matchItem.away_score > 0);

						if (ubet.bet_odd_name == OddConst.Yes) {
							ubet.bet_result = bothToScore ? SportUserBetModelConst.BetResult.Won : SportUserBetModelConst.BetResult.Losed;
						}
						else if (ubet.bet_odd_name == OddConst.No) {
							ubet.bet_result = bothToScore ? SportUserBetModelConst.BetResult.Losed : SportUserBetModelConst.BetResult.Won;
						}
					}
					break;
				}
				case MarketConst.HomeTotals: {
					var matchEnded = matchItem.cur_play_time >= FulltimeSeconds;

					// For eg,. over_2.5
					var arr = ubet.bet_odd_name.Split('_');
					var odd_name = arr[0];
					var pivot = arr[1].ParseDecimalDk();

					if (odd_name == OddConst.Over) {
						if (matchItem.home_score > pivot) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Won;
						}
						else if (matchEnded) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Losed;
						}
					}
					else if (odd_name == OddConst.Under) {
						if (matchItem.home_score > pivot) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Losed;
						}
						else if (matchEnded) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Won;
						}
					}
					break;
				}
				case MarketConst.HomeAsianTotals: {
					break;
				}
				case MarketConst.HomeHandicaps: {
					break;
				}
				case MarketConst.AwayTotals: {
					var matchEnded = matchItem.cur_play_time >= FulltimeSeconds;

					// For eg,. over_2.5
					var arr = ubet.bet_odd_name.Split('_');
					var odd_name = arr[0];
					var pivot = arr[1].ParseDecimalDk();

					if (odd_name == OddConst.Over) {
						if (matchItem.away_score > pivot) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Won;
						}
						else if (matchEnded) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Losed;
						}
					}
					else if (odd_name == OddConst.Under) {
						if (matchItem.away_score > pivot) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Losed;
						}
						else if (matchEnded) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Won;
						}
					}
					break;
				}
				case MarketConst.AwayAsianTotals: {
					break;
				}
				case MarketConst.AwayHandicaps: {
					break;
				}
				case MarketConst.Totals: {
					var matchEnded = matchItem.cur_play_time >= FulltimeSeconds;

					// For eg,. over_2.5, under_1.5
					var arr = ubet.bet_odd_name.Split('_');
					var odd_name = arr[0];
					var pivot = arr[1].ParseIntDk();

					var totalScore = matchItem.home_score + matchItem.away_score;

					if (odd_name == OddConst.Over) {
						if (totalScore > pivot) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Won;
						}
						else if (matchEnded) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Losed;
						}
					}
					else if (odd_name == OddConst.Under) {
						if (totalScore > pivot) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Losed;
						}
						else if (matchEnded) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Won;
						}
					}
					break;
				}
				case MarketConst.AsianTotals: {
					var matchEnded = matchItem.cur_play_time >= FulltimeSeconds;

					// For eg,. over_2.5, under_1.5
					var arr = ubet.bet_odd_name.Split('_');
					var odd_name = arr[0];
					var pivot = arr[1].ParseIntDk();

					var totalScore = matchItem.home_score + matchItem.away_score;

					if (odd_name == OddConst.Over) {
						if (totalScore > pivot) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Won;
						}
						else if (matchEnded) {
							if (totalScore == pivot) {
								ubet.bet_result = SportUserBetModelConst.BetResult.Draw;
							}
							else if (totalScore < pivot) {
								ubet.bet_result = SportUserBetModelConst.BetResult.Losed;
							}
						}
					}
					else if (odd_name == OddConst.Under) {
						if (totalScore > pivot) {
							ubet.bet_result = SportUserBetModelConst.BetResult.Losed;
						}
						else if (matchEnded) {
							if (totalScore == pivot) {
								ubet.bet_result = SportUserBetModelConst.BetResult.Draw;
							}
							else if (totalScore < pivot) {
								ubet.bet_result = SportUserBetModelConst.BetResult.Won;
							}
						}
					}
					break;
				}
				default: {
					throw new AppSystemException($"Invalid market name: {ubet.bet_market_name}");
				}
			}

			// Request send coin-reward to the winner.
			if (ubet.bet_result == SportUserBetModelConst.BetResult.Won) {
				this.dbContext.sportWinnerBetRewardTxs.Attach(new() {
					tx_status = SportWinnerBetRewardTxModelConst.TxStatus.RequestSubmitToChain,
					sport_user_bet_id = ubet.id,
					sender_address = sysAdaWalletAddress,
					receiver_address = ubet.reward_address,
					reward_ada_amount = ubet.bet_ada_amount * ubet.bet_odd_value,
				});
			}
		}

		await this.dbContext.SaveChangesAsync();
	}
}
