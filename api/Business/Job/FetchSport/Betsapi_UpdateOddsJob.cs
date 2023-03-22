namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class Betsapi_UpdateOddsJob : BaseJob {
	private const string JOB_NAME = nameof(Betsapi_UpdateOddsJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<Betsapi_UpdateOddsJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 /3 * * * ?") // Up to 150 req (100 live + 10 upcoming)
			.WithDescription(JOB_NAME)
		);
	}

	private readonly ILogger<Betsapi_UpdateOddsJob> logger;
	private readonly BetsapiRepo betsapiRepo;

	public Betsapi_UpdateOddsJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<Betsapi_UpdateOddsJob> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.betsapiRepo = betsapiRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		// We prior to matches that has long time from previous updated
		var sysLiveMatches = await this.dbContext.sportMatches
			.Where(m => m.status == SportMatchModelConst.TimeStatus.InPlay)
			.OrderBy(m => m.updated_at)
			.ToArrayAsync()
		;

		// Only take at most 1/10 of live match count
		var sysUpcomingMatches = await this.dbContext.sportMatches
			.Where(m => m.status == SportMatchModelConst.TimeStatus.Upcoming)
			.OrderBy(m => m.updated_at)
			.Take(Math.Max(1, sysLiveMatches.Length / 10))
			.ToArrayAsync()
		;

		var sysMatches = new List<SportMatchModel>(sysLiveMatches.Length + sysUpcomingMatches.Length);
		sysMatches.AddRange(sysLiveMatches);
		sysMatches.AddRange(sysUpcomingMatches);

		foreach (var sysMatch in sysMatches) {
			var apiResult = await this.betsapiRepo.FetchMatchOddsSummary(sysMatch.ref_betsapi_match_id);
			if (apiResult is null || apiResult.failed) {
				continue;
			}

			// Update markets of the match.
			// Each odd should NOT exceed 100 to avoid huge payment from system.
			var sysMatchMarkets = sysMatch.markets == null ? new() : (DkJsons.ToObj<List<Market>>(sysMatch.markets) ?? new());
			var name2market = sysMatchMarkets.ToDictionary(m => m.name);

			var (homeOdd, drawOdd, awayOdd) = this._UpdateOdd_MainFullTime(name2market, apiResult);
			this._UpdateOdd_DoubleChance(homeOdd, drawOdd, awayOdd, name2market, apiResult);

			// Write back markets
			sysMatch.markets = DkJsons.ToJson(name2market.Select(m => m.Value).ToArray());
		}

		// Save all matches
		await this.dbContext.SaveChangesAsync();
	}

	/// Update odds by choose lowest result from providers.
	/// Market 1x2 is fulltime result, named as 1_1 (see https://betsapi.com/docs/events/odds.html).
	/// See https://betsapi.com/docs/events/odds.html to get better odd-supported providers.
	private (decimal, decimal, decimal) _UpdateOdd_MainFullTime(
		Dictionary<string, Market> name2market,
		Betsapi_OddsSummaryData apiResult
	) {
		var homeOdd = AppConst.ODD_VALUE_INFINITY;
		var drawOdd = AppConst.ODD_VALUE_INFINITY;
		var awayOdd = AppConst.ODD_VALUE_INFINITY;
		var bookmaker = apiResult.results;

		// Bookmaker at top will result to higher reliable odd and time.
		var odd_1xbet = bookmaker._1XBet?.odds?.end?._1_1;
		if (odd_1xbet != null) {
			if (odd_1xbet.home_od != null) {
				homeOdd = Math.Min(homeOdd, odd_1xbet.home_od.ParseDecimalDk());
			}
			if (odd_1xbet.draw_od != null) {
				drawOdd = Math.Min(drawOdd, odd_1xbet.draw_od.ParseDecimalDk());
			}
			if (odd_1xbet.away_od != null) {
				awayOdd = Math.Min(awayOdd, odd_1xbet.away_od.ParseDecimalDk());
			}
		}

		var odd_10bet = bookmaker._10Bet?.odds?.end?._1_1;
		if (odd_10bet != null) {
			if (odd_10bet.home_od != null) {
				homeOdd = Math.Min(homeOdd, odd_10bet.home_od.ParseDecimalDk());
			}
			if (odd_10bet.draw_od != null) {
				drawOdd = Math.Min(drawOdd, odd_10bet.draw_od.ParseDecimalDk());
			}
			if (odd_10bet.away_od != null) {
				awayOdd = Math.Min(awayOdd, odd_10bet.away_od.ParseDecimalDk());
			}
		}

		var odd_365 = bookmaker.Bet365?.odds?.end?._1_1;
		if (odd_365 != null) {
			if (odd_365.home_od != null) {
				homeOdd = Math.Min(homeOdd, odd_365.home_od.ParseDecimalDk());
			}
			if (odd_365.draw_od != null) {
				drawOdd = Math.Min(drawOdd, odd_365.draw_od.ParseDecimalDk());
			}
			if (odd_365.away_od != null) {
				awayOdd = Math.Min(awayOdd, odd_365.away_od.ParseDecimalDk());
			}
		}

		var odd_bwin = bookmaker.BWin?.odds?.end?._1_1;
		if (odd_bwin != null) {
			if (odd_bwin.home_od != null) {
				homeOdd = Math.Min(homeOdd, odd_bwin.home_od.ParseDecimalDk());
			}
			if (odd_bwin.draw_od != null) {
				drawOdd = Math.Min(drawOdd, odd_bwin.draw_od.ParseDecimalDk());
			}
			if (odd_bwin.away_od != null) {
				awayOdd = Math.Min(awayOdd, odd_bwin.away_od.ParseDecimalDk());
			}
		}

		var odd_unibet = bookmaker.UniBet?.odds?.end?._1_1;
		if (odd_unibet != null) {
			if (odd_unibet.home_od != null) {
				homeOdd = Math.Min(homeOdd, odd_unibet.home_od.ParseDecimalDk());
			}
			if (odd_unibet.draw_od != null) {
				drawOdd = Math.Min(drawOdd, odd_unibet.draw_od.ParseDecimalDk());
			}
			if (odd_unibet.away_od != null) {
				awayOdd = Math.Min(awayOdd, odd_unibet.away_od.ParseDecimalDk());
			}
		}

		// Mark odd as NULL for infinity odds.
		if (homeOdd == AppConst.ODD_VALUE_INFINITY) {
			homeOdd = AppConst.ODD_VALUE_NULL;
		}
		if (drawOdd == AppConst.ODD_VALUE_INFINITY) {
			drawOdd = AppConst.ODD_VALUE_NULL;
		}
		if (awayOdd == AppConst.ODD_VALUE_INFINITY) {
			awayOdd = AppConst.ODD_VALUE_NULL;
		}

		// Remove if the market is unavailable.
		if (homeOdd == AppConst.ODD_VALUE_NULL && drawOdd == AppConst.ODD_VALUE_NULL && awayOdd == AppConst.ODD_VALUE_NULL) {
			name2market.Remove(MarketConst.MainFullTime);
			return (homeOdd, drawOdd, awayOdd);
		}

		// At this time, each odd will be valid value.
		homeOdd = homeOdd.RoundOddAsFloorDk();
		drawOdd = drawOdd.RoundOddAsFloorDk();
		awayOdd = awayOdd.RoundOddAsFloorDk();

		// Force all big odds (> 100) to 100
		if (homeOdd > AppConst.ODD_VALUE_MAX_INCLUSIVE) {
			homeOdd = AppConst.ODD_VALUE_MAX_INCLUSIVE;
		}
		if (drawOdd > AppConst.ODD_VALUE_MAX_INCLUSIVE) {
			drawOdd = AppConst.ODD_VALUE_MAX_INCLUSIVE;
		}
		if (awayOdd > AppConst.ODD_VALUE_MAX_INCLUSIVE) {
			awayOdd = AppConst.ODD_VALUE_MAX_INCLUSIVE;
		}

		// Force all small odds (<=1) to 0
		if (homeOdd <= AppConst.ODD_VALUE_MIN_EXCLUSIVE) {
			homeOdd = AppConst.ODD_VALUE_NULL;
		}
		if (drawOdd <= AppConst.ODD_VALUE_MIN_EXCLUSIVE) {
			drawOdd = AppConst.ODD_VALUE_NULL;
		}
		if (awayOdd <= AppConst.ODD_VALUE_MIN_EXCLUSIVE) {
			awayOdd = AppConst.ODD_VALUE_NULL;
		}

		// Remove if the market is unavailable.
		if (homeOdd == AppConst.ODD_VALUE_NULL && drawOdd == AppConst.ODD_VALUE_NULL && awayOdd == AppConst.ODD_VALUE_NULL) {
			name2market.Remove(MarketConst.MainFullTime);
			return (homeOdd, drawOdd, awayOdd);
		}

		// Upsert 1x2 odd
		var market_1x2 = name2market.GetValueOrDefault(MarketConst.MainFullTime);
		if (market_1x2 is null) {
			market_1x2 = name2market[MarketConst.MainFullTime] = new() { name = MarketConst.MainFullTime, odds = new() };
		}
		market_1x2.odds.Clear();
		market_1x2.odds.AddRange(new Odd[] {
			new() { name = OddConst.HomeWin, value = homeOdd, suspend = (homeOdd == AppConst.ODD_VALUE_NULL) },
			new() { name = OddConst.Draw, value = drawOdd , suspend = (drawOdd == AppConst.ODD_VALUE_NULL) },
			new() { name = OddConst.AwayWin, value =awayOdd, suspend = (awayOdd == AppConst.ODD_VALUE_NULL) },
		});

		return (homeOdd, drawOdd, awayOdd);
	}

	private void _UpdateOdd_DoubleChance(
		decimal _home, decimal _draw, decimal _away,
		Dictionary<string, Market> name2market,
		Betsapi_OddsSummaryData apiResult
	) {
		// Remove dbc if 1x2 unavailable
		if (_home == AppConst.ODD_VALUE_NULL && _draw == AppConst.ODD_VALUE_NULL && _away == AppConst.ODD_VALUE_NULL) {
			name2market.Remove(MarketConst.DoubleChance);
			return;
		}

		// Get or Create dbc
		var market_dbc = name2market.GetValueOrDefault(MarketConst.DoubleChance);
		if (market_dbc is null) {
			market_dbc = name2market[MarketConst.DoubleChance] = new() { name = MarketConst.DoubleChance, odds = new() };
		}

		var homeOrDraw = 0m;
		var homeOrAway = 0m;
		var awayOrDraw = 0m;

		// 必勝。ある比率が１以下。
		if (_home == AppConst.ODD_VALUE_NULL) {
			homeOrDraw = AppConst.ODD_VALUE_NULL;
			homeOrAway = AppConst.ODD_VALUE_NULL;
			awayOrDraw = (_draw + _away) / 5;
		}
		else if (_draw == AppConst.ODD_VALUE_NULL) {
			homeOrDraw = AppConst.ODD_VALUE_NULL;
			awayOrDraw = AppConst.ODD_VALUE_NULL;
			homeOrAway = (_home + _away) / 5;
		}
		else if (_away == AppConst.ODD_VALUE_NULL) {
			awayOrDraw = AppConst.ODD_VALUE_NULL;
			homeOrAway = AppConst.ODD_VALUE_NULL;
			homeOrDraw = (_home + _draw) / 5;
		}
		// 必勝でない。全部が１超え。
		else {
			homeOrDraw = 1 + ((Math.Min(_home, _draw) + Math.Abs(_home - _draw) / 10 - 1) / 5);
			homeOrAway = 1 + ((Math.Min(_home, _away) + Math.Abs(_home - _away) / 10 - 1) / 5);
			awayOrDraw = 1 + ((Math.Min(_away, _draw) + Math.Abs(_away - _draw) / 10 - 1) / 5);

			// Round to 2 decimal first
			homeOrDraw = homeOrDraw.RoundOddAsFloorDk();
			homeOrAway = homeOrAway.RoundOddAsFloorDk();
			awayOrDraw = awayOrDraw.RoundOddAsFloorDk();

			// Force all big odds (> 100) to 100
			if (homeOrDraw > AppConst.ODD_VALUE_MAX_INCLUSIVE) {
				homeOrDraw = AppConst.ODD_VALUE_MAX_INCLUSIVE;
			}
			if (homeOrAway > AppConst.ODD_VALUE_MAX_INCLUSIVE) {
				homeOrAway = AppConst.ODD_VALUE_MAX_INCLUSIVE;
			}
			if (awayOrDraw > AppConst.ODD_VALUE_MAX_INCLUSIVE) {
				awayOrDraw = AppConst.ODD_VALUE_MAX_INCLUSIVE;
			}

			// Force all small odds (<=1) to 0
			if (homeOrDraw <= AppConst.ODD_VALUE_MIN_EXCLUSIVE) {
				homeOrDraw = AppConst.ODD_VALUE_NULL;
			}
			if (homeOrAway <= AppConst.ODD_VALUE_MIN_EXCLUSIVE) {
				homeOrAway = AppConst.ODD_VALUE_NULL;
			}
			if (awayOrDraw <= AppConst.ODD_VALUE_MIN_EXCLUSIVE) {
				awayOrDraw = AppConst.ODD_VALUE_NULL;
			}

			// Remove if the market is unavailable.
			if (homeOrDraw == AppConst.ODD_VALUE_NULL && homeOrAway == AppConst.ODD_VALUE_NULL && awayOrDraw == AppConst.ODD_VALUE_NULL) {
				name2market.Remove(MarketConst.DoubleChance);
				return;
			}
		}

		market_dbc.odds.Clear();
		market_dbc.odds.AddRange(new Odd[] {
			new() { name = OddConst.HomeOrDraw, value = homeOrDraw, suspend = (homeOrDraw == AppConst.ODD_VALUE_NULL) },
			new() { name = OddConst.HomeOrAway, value = homeOrAway, suspend = (homeOrAway == AppConst.ODD_VALUE_NULL) },
			new() { name = OddConst.AwayOrDraw, value = awayOrDraw, suspend = (awayOrDraw == AppConst.ODD_VALUE_NULL) },
		});
	}
}
