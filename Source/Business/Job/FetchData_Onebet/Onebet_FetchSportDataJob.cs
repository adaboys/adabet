namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class Onebet_FetchSportDataJob : BaseJob {
	private const string JOB_NAME = nameof(Onebet_FetchSportDataJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<Onebet_FetchSportDataJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 0 /1 * * ?") // Should fill from left -> right
			.WithDescription(JOB_NAME)
		);
	}

	private readonly ILogger<Onebet_FetchSportDataJob> logger;
	private readonly OnebetRepo onebetRepo;

	public Onebet_FetchSportDataJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<Onebet_FetchSportDataJob> logger,
		OnebetRepo onebetRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.onebetRepo = onebetRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		await this._UpsertFromOnebetAsync();
	}

	private async Task _UpsertFromOnebetAsync() {
		var apiMatches = await this.onebetRepo.FetchAllLiveMatches();
		if (apiMatches is null) {
			return;
		}

		foreach (var apiMatch in apiMatches) {
			var targetMatch = await this.dbContext.sportMatches.FirstOrDefaultAsync(m =>
				m.ref_onebet_home_team_id == apiMatch.team1_id &&
				m.ref_onebet_away_team_id == apiMatch.team2_id
			);

			// Insert new match
			if (targetMatch is null) {
				var targetLeague = await this.dbContext.sportLeagues.FirstOrDefaultAsync(m =>
					m.ref_onebet_league_id == apiMatch.league.league_id
				);
				var homeTeam = await this.dbContext.sportTeams.FirstOrDefaultAsync(m =>
					m.ref_onebet_home_team_id == apiMatch.team1_id
				);
				var awayTeam = await this.dbContext.sportTeams.FirstOrDefaultAsync(m =>
					m.ref_onebet_away_team_id == apiMatch.team2_id
				);

				// Save league and Get id
				if (targetLeague is null) {
					targetLeague = new() {
						sport_id = (await this.dbContext.sports.FirstAsync(m => m.name == MstSportModelConst.Name_Football)).id,
						name = apiMatch.league.name
					};
					this.dbContext.sportLeagues.Attach(targetLeague);
					await this.dbContext.SaveChangesAsync();
				}

				// Save home team and Get id
				if (homeTeam is null) {
					homeTeam = new() {
						name_native = apiMatch.team1,
						name_en = apiMatch.team1,
						//todo//fixme hardcode for now
						flag_image_relative_path = "todo_img_rpath",
						ref_onebet_home_team_id = apiMatch.team1_id
					};
					this.dbContext.sportTeams.Attach(homeTeam);
					await this.dbContext.SaveChangesAsync();
				}

				// Save away team and Get id
				if (awayTeam is null) {
					awayTeam = new() {
						name_native = apiMatch.team2,
						name_en = apiMatch.team2,
						//todo//fixme hardcode for now
						flag_image_relative_path = "todo_img_rpath",
						ref_onebet_away_team_id = apiMatch.team2_id
					};
					this.dbContext.sportTeams.Attach(awayTeam);
					await this.dbContext.SaveChangesAsync();
				}

				// Attach new match
				targetMatch = new() {
					league_id = targetLeague.id,
					home_team_id = homeTeam.id,
					away_team_id = awayTeam.id,

					cur_play_time = (short)(apiMatch.minute * 60 + apiMatch.seconds),

					status = SportMatchModelConst.TimeStatus.InPlay,
					start_at = apiMatch.date_start,

					markets = new(),

					ref_onebet_home_team_id = apiMatch.team1_id,
					ref_onebet_away_team_id = apiMatch.team2_id,
				};

				this.dbContext.sportMatches.Attach(targetMatch);
			}

			// Update match
			targetMatch.home_score = apiMatch.score1;
			targetMatch.away_score = apiMatch.score2;

			// [Upsert markets]
			var onebetMarkets = apiMatch.markets;
			if (onebetMarkets != null) {
				var name2market = targetMatch.markets.ToDictionary(m => m.name);

				// Upsert market
				var market_1x2 = name2market.GetValueOrDefault(MarketConst.MainFullTime);
				if (onebetMarkets.win1 != null || onebetMarkets.winX != null || onebetMarkets.win2 != null) {
					if (market_1x2 is null) {
						market_1x2 = new() { name = MarketConst.MainFullTime, odds = new() };
						name2market.TryAdd(MarketConst.MainFullTime, market_1x2);
					}
					market_1x2.odds.Clear();
					market_1x2.odds.AddRange(new Odd[] {
					new() { name = OddConst.HomeWin, value = onebetMarkets.win1?.v.RoundOddAsFloorDk() ?? 0 },
					new() { name = OddConst.Draw, value = onebetMarkets.winX?.v.RoundOddAsFloorDk() ?? 0 },
					new() { name = OddConst.AwayWin, value = onebetMarkets.win2?.v.RoundOddAsFloorDk() ?? 0 },
				});
				}
				else {
					name2market.Remove(MarketConst.MainFullTime);
				}

				// Upsert market
				var market_bothToScore = name2market.GetValueOrDefault(MarketConst.BothToScore);
				if (onebetMarkets.bothToScore != null && (onebetMarkets.bothToScore.yes != null || onebetMarkets.bothToScore.no != null)) {
					if (market_bothToScore is null) {
						market_bothToScore = new() { name = MarketConst.BothToScore, odds = new() };
						name2market.TryAdd(MarketConst.BothToScore, market_bothToScore);
					}
					market_bothToScore.odds.Clear();
					market_bothToScore.odds.AddRange(new Odd[] {
					new() { name = OddConst.Yes, value = onebetMarkets.bothToScore.yes?.v.RoundOddAsFloorDk() ?? 0 },
					new() { name = OddConst.No, value = onebetMarkets.bothToScore.no?.v.RoundOddAsFloorDk() ?? 0 }
				});
				}
				else {
					name2market.Remove(MarketConst.BothToScore);
				}

				// Upsert market
				var market_totals = name2market.GetValueOrDefault(MarketConst.Totals);
				if (onebetMarkets.totals != null && onebetMarkets.totals.Count > 0) {
					if (market_totals is null) {
						market_totals = new() { name = MarketConst.Totals, odds = new() };
						name2market.TryAdd(MarketConst.Totals, market_totals);
					}
					market_totals.odds.Clear();
					foreach (var item in onebetMarkets.totals) {
						market_totals.odds.Add(new() {
							name = $"{OddConst.Over}_{item.type}",
							value = item.over.v.RoundOddAsFloorDk(),
						});

						market_totals.odds.Add(new() {
							name = $"{OddConst.Under}_{item.type}",
							value = item.under.v.RoundOddAsFloorDk()
						});
					}
				}
				else {
					name2market.Remove(MarketConst.Totals);
				}

				// Upsert market
				var market_asianTotals = name2market.GetValueOrDefault(MarketConst.AsianTotals);
				if (onebetMarkets.totalsAsian != null && onebetMarkets.totalsAsian.Count > 0) {
					if (market_asianTotals is null) {
						market_asianTotals = new() { name = MarketConst.AsianTotals, odds = new() };
						name2market.TryAdd(MarketConst.AsianTotals, market_asianTotals);
					}
					foreach (var item in onebetMarkets.totalsAsian) {
						market_asianTotals.odds.Add(new() {
							name = $"{OddConst.Over}_{item.type}",
							value = item.over.v.RoundOddAsFloorDk(),
						});

						market_asianTotals.odds.Add(new() {
							name = $"{OddConst.Under}_{item.type}",
							value = item.under.v.RoundOddAsFloorDk()
						});
					}
				}
				else {
					name2market.Remove(MarketConst.AsianTotals);
				}

				// Upsert market
				var market_homeTotals = name2market.GetValueOrDefault(MarketConst.HomeTotals);
				if (onebetMarkets.totals1 != null && onebetMarkets.totals1.Count > 0) {
					if (market_homeTotals is null) {
						market_homeTotals = new() { name = MarketConst.HomeTotals, odds = new() };
						name2market.TryAdd(MarketConst.HomeTotals, market_homeTotals);
					}
					foreach (var item in onebetMarkets.totals1) {
						market_homeTotals.odds.Add(new() {
							name = $"{OddConst.Over}_{item.type}",
							value = item.over.v.RoundOddAsFloorDk(),
						});

						market_homeTotals.odds.Add(new() {
							name = $"{OddConst.Under}_{item.type}",
							value = item.under.v.RoundOddAsFloorDk()
						});
					}
				}
				else {
					name2market.Remove(MarketConst.HomeTotals);
				}

				// Upsert market
				var market_homeAsianTotals = name2market.GetValueOrDefault(MarketConst.HomeAsianTotals);
				if (onebetMarkets.totals1Asian != null && onebetMarkets.totals1Asian.Count > 0) {
					if (market_homeAsianTotals is null) {
						market_homeAsianTotals = new() { name = MarketConst.HomeAsianTotals, odds = new() };
						name2market.TryAdd(MarketConst.HomeAsianTotals, market_homeAsianTotals);
					}
					market_homeAsianTotals.odds.Clear();
					foreach (var item in onebetMarkets.totals1Asian) {
						market_homeAsianTotals.odds.Add(new() {
							name = $"{OddConst.Over}_{item.type}",
							value = item.over.v.RoundOddAsFloorDk()
						});

						market_homeAsianTotals.odds.Add(new() {
							name = $"{OddConst.Under}_{item.type}",
							value = item.under.v.RoundOddAsFloorDk()
						});
					}
				}

				// Upsert market
				var market_awayTotals = name2market.GetValueOrDefault(MarketConst.AwayTotals);
				if (onebetMarkets.totals2 != null && onebetMarkets.totals2.Count > 0) {
					if (market_awayTotals is null) {
						market_awayTotals = new() { name = MarketConst.AwayTotals, odds = new() };
						name2market.TryAdd(MarketConst.AwayTotals, market_awayTotals);
					}
					market_awayTotals.odds.Clear();
					foreach (var item in onebetMarkets.totals2) {
						market_awayTotals.odds.Add(new() {
							name = $"{OddConst.Over}_{item.type}",
							value = item.over.v.RoundOddAsFloorDk()
						});

						market_awayTotals.odds.Add(new() {
							name = $"{OddConst.Under}_{item.type}",
							value = item.under.v.RoundOddAsFloorDk()
						});
					}
				}

				// Upsert market
				var market_awayAsianTotals = name2market.GetValueOrDefault(MarketConst.AwayAsianTotals);
				if (onebetMarkets.totals2Asian != null && onebetMarkets.totals2Asian.Count > 0) {
					if (market_awayAsianTotals is null) {
						market_awayAsianTotals = new() { name = MarketConst.AwayAsianTotals, odds = new() };
						name2market.TryAdd(MarketConst.AwayAsianTotals, market_awayAsianTotals);
					}
					market_awayAsianTotals.odds.Clear();
					foreach (var item in onebetMarkets.totals2Asian) {
						market_awayAsianTotals.odds.Add(new() {
							name = $"{OddConst.Over}_{item.type}",
							value = item.over.v.RoundOddAsFloorDk()
						});

						market_awayAsianTotals.odds.Add(new() {
							name = $"{OddConst.Under}_{item.type}",
							value = item.under.v.RoundOddAsFloorDk()
						});
					}
				}

				// Upsert market
				var market_homeHandicaps = name2market.GetValueOrDefault(MarketConst.HomeHandicaps);
				if (onebetMarkets.handicaps1 != null && onebetMarkets.handicaps1.Count > 0) {
					if (market_homeHandicaps is null) {
						market_homeHandicaps = new() { name = MarketConst.HomeHandicaps, odds = new() };
						name2market.TryAdd(MarketConst.HomeHandicaps, market_homeHandicaps);
					}
					market_homeHandicaps.odds.Clear();
					foreach (var item in onebetMarkets.handicaps1) {
						market_homeHandicaps.odds.Add(new() {
							name = $"{OddConst.Handicap}_{item.type}",
							value = item.v.RoundOddAsFloorDk()
						});
					}
				}

				// Upsert market
				var market_awayHandicaps = name2market.GetValueOrDefault(MarketConst.AwayHandicaps);
				if (onebetMarkets.handicaps2 != null && onebetMarkets.handicaps2.Count > 0) {
					if (market_awayHandicaps is null) {
						market_awayHandicaps = new() { name = MarketConst.AwayHandicaps, odds = new() };
						name2market.TryAdd(MarketConst.AwayHandicaps, market_awayHandicaps);
					}
					market_awayHandicaps.odds.Clear();
					foreach (var item in onebetMarkets.handicaps2) {
						market_awayHandicaps.odds.Add(new() {
							name = $"{OddConst.Handicap}_{item.type}",
							value = item.v.RoundOddAsFloorDk()
						});
					}
				}

				// Update back to match.markets
				targetMatch.markets = name2market.Select(m => m.Value).ToList();
			}
		}

		// Save all matches
		await this.dbContext.SaveChangesAsync();

		//todo should notify all clients if some match data has changed??
		//todo should clear redis cache since we will cache query result at other apis?
	}
}
