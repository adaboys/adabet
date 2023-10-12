namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class UpdateOdds_BasketballJob_Betsapi : BaseJob<UpdateOdds_BasketballJob_Betsapi> {
	private const string JOB_NAME = nameof(UpdateOdds_BasketballJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateOdds_BasketballJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /1 * * * ?" : // Should at 30s
				"0 /5 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	protected readonly BetsapiRepo betsapiRepo;

	public UpdateOdds_BasketballJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateOdds_BasketballJob_Betsapi> logger,
		MailComponent mailComponent,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent) {
		this.betsapiRepo = betsapiRepo;
	}

	/// It consumes at most 300 requests.
	public override async Task Run(IJobExecutionContext context) {
		// We prior to matches that has long time from previous updated
		var queryLiveMatches =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			where _league.sport_id == MstSportModelConst.Id_Basketball
			where _match.status == SportMatchModelConst.TimeStatus.InPlay

			// We prior to matches that has long time from previous updated
			orderby _match.updated_at

			select new {
				_match
			}
		;
		var liveMatches = await queryLiveMatches.Select(m => m._match).Take(300).ToArrayAsync();

		// Get upcoming matches
		var queryUpcomingMatches =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			where _league.sport_id == MstSportModelConst.Id_Basketball
			where _match.status == SportMatchModelConst.TimeStatus.Upcoming

			// We prior to matches that has long time from previous updated
			orderby _match.updated_at

			select new {
				_match
			}
		;
		var upcomingMatches = await queryUpcomingMatches.Select(m => m._match).Take(30).ToArrayAsync();

		// Merge matches and sync
		var sysMatches = new List<SportMatchModel>(100);
		sysMatches.AddRange(liveMatches);
		sysMatches.AddRange(upcomingMatches);

		await this.UpdateMatchOddsAsync(sysMatches);
	}

	protected async Task UpdateMatchOddsAsync(List<SportMatchModel> sysMatches) {
		if (sysMatches.Count == 0) {
			return;
		}

		// Use LinQ will deferrer (lazy) start task. If we call ToArray() or ToList() on deferredTasks,
		// it causes tasks be executed immediately.
		var deferredTasks = sysMatches.Select(sysMatch => _UpdateMatchOddAsync(sysMatch));

		// Run tasks parallel.
		// Each task maybe run at different thread, we need take care of concurency when interact with dbContext !
		await Task.WhenAll(deferredTasks);

		// Save changes
		await this.dbContext.SaveChangesAsync();
	}

	private async Task _UpdateMatchOddAsync(SportMatchModel sysMatch) {
		var apiResult = await this.betsapiRepo.FetchMatchOddsSummary<Betsapi_BasketballOddsSummaryData>(sysMatch.ref_betsapi_match_id);
		if (apiResult is null || apiResult.failed) {
			this.logger.ErrorDk(this, $"Fetch Basketball match odds failed", $"sysMatchId: {sysMatch.id}, apiResult: {apiResult}");

			// Try remap match id
			await BetsapiHelper.RemapMatchIdAsync(sysMatch: sysMatch, betsapiRepo: this.betsapiRepo, logger: logger, caller: this);

			return;
		}

		// Update markets of the match.
		// Each odd should NOT exceed 100 to avoid huge payment from system.
		var sysMatchMarkets = sysMatch.markets == null ? new() : (DkJsons.ToObj<List<Market>>(sysMatch.markets) ?? new());
		var name2market = sysMatchMarkets.ToDictionary(m => m.name);

		this._UpdateOdd_MainFullTime(name2market, apiResult);

		// Write back markets
		sysMatch.markets = DkJsons.ToJson(name2market.Values);
	}

	/// Update odds by choose lowest result from providers.
	/// Market 1x2 is fulltime result, named as 1_1 (see https://betsapi.com/docs/events/odds.html).
	/// See https://betsapi.com/docs/events/odds.html to get better odd-supported providers.
	private void _UpdateOdd_MainFullTime(
		Dictionary<string, Market> name2market,
		Betsapi_BasketballOddsSummaryData apiResult
	) {
		var homeOdd = AppConst.ODD_VALUE_INFINITY;
		var awayOdd = AppConst.ODD_VALUE_INFINITY;
		var bookmaker = apiResult.results;

		// Bookmaker at top will result to higher reliable odd and time.
		var odd_365 = bookmaker.Bet365?.odds?.end?._18_1 ?? bookmaker.Bet365?.odds?.kickoff?._18_1 ?? bookmaker.Bet365?.odds?.start?._18_1;
		if (odd_365 != null) {
			if (odd_365.home_od != null) {
				homeOdd = Math.Min(homeOdd, odd_365.home_od.ParseDecimalDk());
			}
			if (odd_365.away_od != null) {
				awayOdd = Math.Min(awayOdd, odd_365.away_od.ParseDecimalDk());
			}
		}

		var odd_betAtHome = bookmaker.BetAtHome?.odds?.end?._18_1 ?? bookmaker.BetAtHome?.odds?.kickoff?._18_1 ?? bookmaker.BetAtHome?.odds?.start?._18_1;
		if (odd_betAtHome != null) {
			if (odd_betAtHome.home_od != null) {
				homeOdd = Math.Min(homeOdd, odd_betAtHome.home_od.ParseDecimalDk());
			}
			if (odd_betAtHome.away_od != null) {
				awayOdd = Math.Min(awayOdd, odd_betAtHome.away_od.ParseDecimalDk());
			}
		}

		// Mark odd as NULL for infinity odds.
		if (homeOdd == AppConst.ODD_VALUE_INFINITY) {
			homeOdd = AppConst.ODD_VALUE_NULL;
		}
		if (awayOdd == AppConst.ODD_VALUE_INFINITY) {
			awayOdd = AppConst.ODD_VALUE_NULL;
		}

		// Remove if the market is unavailable.
		if (homeOdd == AppConst.ODD_VALUE_NULL && awayOdd == AppConst.ODD_VALUE_NULL) {
			name2market.Remove(MarketConst.MainFullTime);
			return;
		}

		// At this time, each odd will be valid value.
		homeOdd = homeOdd.RoundOddDk();
		awayOdd = awayOdd.RoundOddDk();

		// Force all big odds (> 100) to 100
		if (homeOdd > AppConst.ODD_VALUE_MAX_INCLUSIVE) {
			homeOdd = AppConst.ODD_VALUE_MAX_INCLUSIVE;
		}
		if (awayOdd > AppConst.ODD_VALUE_MAX_INCLUSIVE) {
			awayOdd = AppConst.ODD_VALUE_MAX_INCLUSIVE;
		}

		// Force all small odds (<=1) to 0
		if (homeOdd <= AppConst.ODD_VALUE_MIN_EXCLUSIVE) {
			homeOdd = AppConst.ODD_VALUE_NULL;
		}
		if (awayOdd <= AppConst.ODD_VALUE_MIN_EXCLUSIVE) {
			awayOdd = AppConst.ODD_VALUE_NULL;
		}

		// Remove if the market is unavailable.
		if (homeOdd == AppConst.ODD_VALUE_NULL && awayOdd == AppConst.ODD_VALUE_NULL) {
			name2market.Remove(MarketConst.MainFullTime);
			return;
		}

		// Upsert 1x2 odd
		var market_1x2 = name2market.GetValueOrDefault(MarketConst.MainFullTime);
		if (market_1x2 is null) {
			market_1x2 = name2market[MarketConst.MainFullTime] = new() { name = MarketConst.MainFullTime, odds = new() };
		}
		market_1x2.odds.Clear();
		market_1x2.odds.AddRange(new Odd[] {
			new() { name = OddConst.HomeWin, value = homeOdd, suspend = (homeOdd == AppConst.ODD_VALUE_NULL) },
			new() { name = OddConst.AwayWin, value = awayOdd, suspend = (awayOdd == AppConst.ODD_VALUE_NULL) },
		});
	}
}
