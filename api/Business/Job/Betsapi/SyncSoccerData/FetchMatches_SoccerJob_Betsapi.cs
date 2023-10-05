namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class FetchLiveMatches_ForSoccerJob_Betsapi : Base_FetchSoccerMatchesJob_Betsapi<FetchLiveMatches_ForSoccerJob_Betsapi> {
	private const string JOB_NAME = nameof(FetchLiveMatches_ForSoccerJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<FetchLiveMatches_ForSoccerJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /1 * * * ?" : // Should at 30s
				"0 /5 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public FetchLiveMatches_ForSoccerJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<FetchLiveMatches_ForSoccerJob_Betsapi> logger,
		MailComponent mailComponent,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent, betsapiRepo: betsapiRepo) {
	}

	/// It consumes just 1 request per call.
	public override async Task Run(IJobExecutionContext context) {
		// Onetime fetch
		var apiResult = await this.betsapiRepo.FetchInplayMatches<Betsapi_SoccerMatchesData>(MstSportModelConst.Id_Soccer_betsapi);
		if (apiResult is null || apiResult.failed) {
			return;
		}

		var apiMatches = apiResult.results;

		foreach (var apiMatch in apiMatches) {
			var sysMatches = await this.dbContext.sportMatches.Where(m => m.ref_betsapi_match_id == apiMatch.id).ToArrayAsync();

			// Register new match with apiMatch info
			if (sysMatches.Length == 0) {
				var leagueName = apiMatch.league.name;
				var mySportId = (leagueName != null && leagueName.StartsWithDk("Esoccer")) ?
					MstSportModelConst.Id_Esoccer :
					MstSportModelConst.Id_Soccer
				;
				sysMatches = new SportMatchModel[] {
					await this.RegisterNewMatchAsync(
						sport_id: mySportId,
						apiMatch: apiMatch,
						timeStatus: SportMatchModelConst.TimeStatus.InPlay
					)
				};
			}

			// Update matches info
			foreach (var sysMatch in sysMatches) {
				// Update current play time
				var timer = apiMatch.timer;
				if (timer != null) {
					sysMatch.timer = (short)(timer.tm * 60 + timer.ts);
					sysMatch.timer_break = timer.tt == "1";
					sysMatch.timer_injury = (short)(timer.ta * 60);
				}

				// Update current score
				if (apiMatch.ss != null) {
					var scores = apiMatch.ss.Split('-');
					if (scores.Length == 2) {
						sysMatch.home_score = scores[0].ParseShortDk();
						sysMatch.away_score = scores[1].ParseShortDk();
					}
				}
			}
		}

		// Save all matches
		await this.dbContext.SaveChangesAsync();
	}
}

[DisallowConcurrentExecution]
public class FetchUpcomingMatches_ForSoccerJob_Betsapi : Base_FetchSoccerMatchesJob_Betsapi<FetchUpcomingMatches_ForSoccerJob_Betsapi> {
	private const string JOB_NAME = nameof(FetchUpcomingMatches_ForSoccerJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<FetchUpcomingMatches_ForSoccerJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 0 1 /1 * ?") // Every day
			.WithDescription(JOB_NAME)
		);
	}

	public FetchUpcomingMatches_ForSoccerJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<FetchUpcomingMatches_ForSoccerJob_Betsapi> logger,
		BetsapiRepo betsapiRepo,
		MailComponent mailComponent
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent, betsapiRepo: betsapiRepo) {
	}

	/// It consumes about 100 requests. And max 300 requests.
	public override async Task Run(IJobExecutionContext context) {
		var now = DateTime.UtcNow;

		// Fetch 3 days from today, that is: 今日、明日、明後日
		for (var index = 1; index <= 3; ++index) {
			// Padding 0: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
			var day = $"{(now.Year):D4}{(now.Month):D2}{(now.Day):D2}";

			// Recursive fetch
			await this._RecursiveFetchUpcomingMatches(day: day, page: 1);

			now = now.AddDays(1);
		}
	}

	private async Task _RecursiveFetchUpcomingMatches(string day, int page) {
		// Note that, betsapi requires page must <= 100
		var apiResult = await this.betsapiRepo.FetchUpcomingMatches<Betsapi_SoccerMatchesData>(MstSportModelConst.Id_Soccer_betsapi, day, page);
		if (apiResult is null || apiResult.failed) {
			this.logger.ErrorDk(this, "Fetch upcoming soccer failed. Api response: {@data}", apiResult);
			return;
		}

		var apiMatches = apiResult.results;

		foreach (var apiMatch in apiMatches) {
			var hasSysMatch = await this.dbContext.sportMatches.AnyAsync(m => m.ref_betsapi_match_id == apiMatch.id);

			// Register new match with its info (league, team,...)
			if (!hasSysMatch) {
				var leagueName = apiMatch.league.name;
				var mySportId = (leagueName != null && leagueName.StartsWithDk("Esoccer")) ?
					MstSportModelConst.Id_Esoccer :
					MstSportModelConst.Id_Soccer
				;
				await this.RegisterNewMatchAsync(
					sport_id: mySportId,
					apiMatch: apiMatch,
					timeStatus: SportMatchModelConst.TimeStatus.InPlay
				);
			}
		}

		// Save all matches
		await this.dbContext.SaveChangesAsync();

		// Continuous fetch until reach to final page !
		if (apiResult.pager.page * apiResult.pager.per_page < apiResult.pager.total) {
			await this._RecursiveFetchUpcomingMatches(day, apiResult.pager.page + 1);
		}
	}
}

public abstract class Base_FetchSoccerMatchesJob_Betsapi<T> : BaseJob<T> where T : class {
	protected readonly BetsapiRepo betsapiRepo;

	public Base_FetchSoccerMatchesJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<T> logger,
		MailComponent mailComponent,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent) {
		this.betsapiRepo = betsapiRepo;
	}

	protected async Task<SportMatchModel> RegisterNewMatchAsync(
		int sport_id,
		Betsapi_SoccerMatchesData.Result apiMatch,
		SportMatchModelConst.TimeStatus timeStatus
	) {
		var targetLeague = await this.dbContext.sportLeagues.FirstOrDefaultAsync(m =>
			m.ref_betsapi_league_id == apiMatch.league.id
		);
		var homeTeam = await this.dbContext.sportTeams.FirstOrDefaultAsync(m =>
			m.ref_betsapi_home_team_id == apiMatch.home.id
		);
		var awayTeam = await this.dbContext.sportTeams.FirstOrDefaultAsync(m =>
			m.ref_betsapi_away_team_id == apiMatch.away.id
		);

		// Save league and Get id
		if (targetLeague is null) {
			targetLeague = new() {
				sport_id = sport_id,
				name = apiMatch.league.name,
				ref_betsapi_league_id = apiMatch.league.id
			};
			this.dbContext.sportLeagues.Attach(targetLeague);
			await this.dbContext.SaveChangesAsync();
		}
		++targetLeague.match_count;

		// Save home team and Get id
		if (homeTeam is null) {
			var image_id = await this.betsapiRepo.CalcTeamImageId(apiMatch.home.image_id);

			homeTeam = new() {
				name = apiMatch.home.name,
				flag_image_name = image_id,
				flag_image_src = SportTeamModelConst.FlagImageSource.Betsapi,
				ref_betsapi_home_team_id = apiMatch.home.id
			};
			this.dbContext.sportTeams.Attach(homeTeam);
			await this.dbContext.SaveChangesAsync();
		}

		// Save away team and Get id
		if (awayTeam is null) {
			var image_id = await this.betsapiRepo.CalcTeamImageId(apiMatch.away.image_id);

			awayTeam = new() {
				name = apiMatch.away.name,
				flag_image_name = image_id,
				flag_image_src = SportTeamModelConst.FlagImageSource.Betsapi,
				ref_betsapi_away_team_id = apiMatch.away.id
			};
			this.dbContext.sportTeams.Attach(awayTeam);
			await this.dbContext.SaveChangesAsync();
		}

		// Attach new match
		var targetMatch = new SportMatchModel() {
			league_id = targetLeague.id,
			home_team_id = homeTeam.id,
			away_team_id = awayTeam.id,

			status = timeStatus,
			start_at = DateTimeOffset.FromUnixTimeSeconds(apiMatch.time).UtcDateTime,

			ref_betsapi_match_id = apiMatch.id,
			ref_betsapi_home_team_id = apiMatch.home.id,
			ref_betsapi_away_team_id = apiMatch.away.id,
		};
		// Mark if the match is esoccer (Esoccer Battle - 8 mins play).
		// And calculate total timer for esoccer.
		if (sport_id == MstSportModelConst.Id_Esoccer) {
			targetMatch.is_esport = true;

			var name2time = apiMatch.league.name.Split('-');
			if (name2time.Length >= 2) {
				var times = name2time.Last().Trim().Split(' ');
				if (times.Length > 0) {
					targetMatch.total_timer = (short)(times[0].Trim().ParseShortDk() * 60);
				}
			}
		}

		this.dbContext.sportMatches.Attach(targetMatch);

		return targetMatch;
	}
}