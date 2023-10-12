namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class UpdateOdds_SoccerJob_Betsapi : BaseJob<UpdateOdds_SoccerJob_Betsapi> {
	private const string JOB_NAME = nameof(UpdateOdds_SoccerJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateOdds_SoccerJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /1 * * * ?" : // Should at 30s
				"0 /5 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	private readonly BetsapiRepo betsapiRepo;

	public UpdateOdds_SoccerJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateOdds_SoccerJob_Betsapi> logger,
		MailComponent mailComponent,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent) {
		this.betsapiRepo = betsapiRepo;
	}

	/// It consumes at most 300 requests.
	public override async Task Run(IJobExecutionContext context) {
		// Get live matches
		var queryLiveMatches =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			// Filter soccer or esoccer since they are same id at betsapi
			where MstSportModelConst.FootbalIds.Contains(_league.sport_id)
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

			// Filter soccer or esoccer since they are same id at betsapi
			where MstSportModelConst.FootbalIds.Contains(_league.sport_id)
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

		await this._UpdateMatchOddsAsync(sysMatches);
	}

	protected async Task _UpdateMatchOddsAsync(List<SportMatchModel> sysMatches) {
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
		var apiResult = await this.betsapiRepo.FetchMatchOddsSummary<Betsapi_SoccerOddsSummaryData>(sysMatch.ref_betsapi_match_id);
		if (apiResult is null || apiResult.failed) {
			this.logger.ErrorDk(this, $"Could not fetch soccer match ({sysMatch.id}:{sysMatch.ref_betsapi_match_id}) odds failed." + " Api response: {@data}", apiResult);

			// Try remap match-id
			await BetsapiHelper.RemapMatchIdAsync(sysMatch: sysMatch, betsapiRepo: this.betsapiRepo, logger: logger, caller: this);

			return;
		}

		// Update markets of the match.
		// Each odd should NOT exceed 100 to avoid huge payment from system.
		var sysMatchMarkets = sysMatch.markets == null ? new() : (DkJsons.ToObj<List<Market>>(sysMatch.markets) ?? new());
		var name2market = sysMatchMarkets.ToDictionary(m => m.name);

		var (homeOdd, drawOdd, awayOdd) = this._UpdateOdd_MainFullTime(name2market, apiResult);
		this._UpdateOdd_DoubleChance(homeOdd, drawOdd, awayOdd, name2market, apiResult);

		// Write back markets
		sysMatch.markets = DkJsons.ToJson(name2market.Values);
	}

	/// Update odds by choose lowest result from providers.
	/// Market 1x2 is fulltime result, named as 1_1 (see https://betsapi.com/docs/events/odds.html).
	/// See https://betsapi.com/docs/events/odds.html to get better odd-supported providers.
	private (decimal, decimal, decimal) _UpdateOdd_MainFullTime(
		Dictionary<string, Market> name2market,
		Betsapi_SoccerOddsSummaryData apiResult
	) {
		var homeOdd = AppConst.ODD_VALUE_INFINITY;
		var drawOdd = AppConst.ODD_VALUE_INFINITY;
		var awayOdd = AppConst.ODD_VALUE_INFINITY;
		var bookmaker = apiResult.results;

		// Bookmaker at top will result to higher reliable odd and time.
		var odd_1xbet = bookmaker._1XBet?.odds?.end?._1_1 ?? bookmaker._1XBet?.odds?.kickoff?._1_1 ?? bookmaker._1XBet?.odds?.start?._1_1;
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

		var odd_10bet = bookmaker._10Bet?.odds?.end?._1_1 ?? bookmaker._10Bet?.odds?.kickoff?._1_1 ?? bookmaker._10Bet?.odds?.start?._1_1;
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

		var odd_365 = bookmaker.Bet365?.odds?.end?._1_1 ?? bookmaker.Bet365?.odds?.kickoff?._1_1 ?? bookmaker.Bet365?.odds?.start?._1_1;
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

		var odd_bwin = bookmaker.BWin?.odds?.end?._1_1 ?? bookmaker.BWin?.odds?.kickoff?._1_1 ?? bookmaker.BWin?.odds?.start?._1_1;
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

		var odd_unibet = bookmaker.UniBet?.odds?.end?._1_1 ?? bookmaker.UniBet?.odds?.kickoff?._1_1 ?? bookmaker.UniBet?.odds?.start?._1_1;
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
		homeOdd = homeOdd.RoundOddDk();
		drawOdd = drawOdd.RoundOddDk();
		awayOdd = awayOdd.RoundOddDk();

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
		Betsapi_SoccerOddsSummaryData apiResult
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
			homeOrDraw = this._CalcDoubleChanceOdd(_home, _draw, _away);
			homeOrAway = this._CalcDoubleChanceOdd(_home, _away, _draw);
			awayOrDraw = this._CalcDoubleChanceOdd(_away, _draw, _home);

			// Round to 2 decimal first
			homeOrDraw = homeOrDraw.RoundOddDk();
			homeOrAway = homeOrAway.RoundOddDk();
			awayOrDraw = awayOrDraw.RoundOddDk();

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

	private decimal _CalcDoubleChanceOdd(decimal odd1, decimal odd2, decimal other) {
		var defaultScale = 5m;
		var diff = other - Math.Max(odd1, odd2);

		if (diff > 0) {
			defaultScale += diff / 10;
		}

		return 1 + ((Math.Min(odd1, odd2) - 1) / defaultScale);
	}
}
