namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class UpdateTennisOdds_LiveJob_Betsapi : Base_UpdateTennisOddsJob_Betsapi<UpdateTennisOdds_LiveJob_Betsapi> {
	private const string JOB_NAME = nameof(UpdateTennisOdds_LiveJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateTennisOdds_LiveJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /1 * * * ?" : // Should run at every minute
				"0 /10 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public UpdateTennisOdds_LiveJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateTennisOdds_LiveJob_Betsapi> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot, logger, betsapiRepo) {
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		// We prior to matches that has long time from previous updated
		var query =
			from _match in this.dbContext.sportMatches
			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			where _league.sport_id == MstSportModelConst.Id_Tennis
			where _match.status == SportMatchModelConst.TimeStatus.InPlay
			orderby _match.updated_at
			select new {
				_match
			}
		;
		var sysMatches = await query.Select(m => m._match).Take(300).ToArrayAsync();

		await this.UpdateMatchOddsAsync(sysMatches);
	}
}

/// Upcoming
[DisallowConcurrentExecution]
public class UpdateTennisOdds_UpcomingJob_Betsapi : Base_UpdateTennisOddsJob_Betsapi<UpdateTennisOdds_UpcomingJob_Betsapi> {
	private const string JOB_NAME = nameof(UpdateTennisOdds_UpcomingJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateTennisOdds_UpcomingJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /2 * * * ?" : // Should run at every minute
				"0 /10 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public UpdateTennisOdds_UpcomingJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateTennisOdds_UpcomingJob_Betsapi> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot, logger, betsapiRepo) {
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		// １０試合ずつを順次に更新していく
		var query =
			from _match in this.dbContext.sportMatches
			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			where _league.sport_id == MstSportModelConst.Id_Tennis
			where _match.status == SportMatchModelConst.TimeStatus.Upcoming
			orderby _match.updated_at
			select new {
				_match
			}
		;
		var sysMatches = await query.Select(m => m._match).Take(10).ToArrayAsync();

		await this.UpdateMatchOddsAsync(sysMatches);
	}
}

public abstract class Base_UpdateTennisOddsJob_Betsapi<T> : BaseJob where T : class {
	protected readonly ILogger<T> logger;
	protected readonly BetsapiRepo betsapiRepo;

	public Base_UpdateTennisOddsJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<T> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.betsapiRepo = betsapiRepo;
	}

	protected async Task UpdateMatchOddsAsync(SportMatchModel[] sysMatches) {
		if (sysMatches.Length == 0) {
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
		var apiResult = await this.betsapiRepo.FetchMatchOddsSummary<Betsapi_TennisOddsSummaryData>(sysMatch.ref_betsapi_match_id);
		if (apiResult is null || apiResult.failed) {
			return;
		}

		// Update markets of the match.
		// Each odd should NOT exceed 100 to avoid huge payment from system.
		var sysMatchMarkets = sysMatch.markets == null ? new() : (DkJsons.ToObj<List<Market>>(sysMatch.markets) ?? new());
		var name2market = sysMatchMarkets.ToDictionary(m => m.name);

		this._UpdateOdd_MainFullTime(name2market, apiResult);

		// Write back markets
		sysMatch.markets = DkJsons.ToJson(name2market.Select(m => m.Value).ToArray());
	}

	/// Update odds by choose lowest result from providers.
	/// Market 1x2 is fulltime result, named as 1_1 (see https://betsapi.com/docs/events/odds.html).
	/// See https://betsapi.com/docs/events/odds.html to get better odd-supported providers.
	private void _UpdateOdd_MainFullTime(
		Dictionary<string, Market> name2market,
		Betsapi_TennisOddsSummaryData apiResult
	) {
		var homeOdd = AppConst.ODD_VALUE_INFINITY;
		var awayOdd = AppConst.ODD_VALUE_INFINITY;
		var bookmaker = apiResult.results;

		// Bookmaker at top will result to higher reliable odd and time.
		var odd_365 = bookmaker.Bet365?.odds?.end?._13_1 ?? bookmaker.Bet365?.odds?.kickoff?._13_1 ?? bookmaker.Bet365?.odds?.start?._13_1;
		if (odd_365 != null) {
			if (odd_365.home_od != null) {
				homeOdd = Math.Min(homeOdd, odd_365.home_od.ParseDecimalDk());
			}
			if (odd_365.away_od != null) {
				awayOdd = Math.Min(awayOdd, odd_365.away_od.ParseDecimalDk());
			}
		}

		var odd_bwin = bookmaker.BWin?.odds?.end?._13_1 ?? bookmaker.BWin?.odds?.kickoff?._13_1 ?? bookmaker.BWin?.odds?.start?._13_1;
		if (odd_bwin != null) {
			if (odd_bwin.home_od != null) {
				homeOdd = Math.Min(homeOdd, odd_bwin.home_od.ParseDecimalDk());
			}
			if (odd_bwin.away_od != null) {
				awayOdd = Math.Min(awayOdd, odd_bwin.away_od.ParseDecimalDk());
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
