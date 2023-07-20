namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class FetchSoccerLiveMatchesJob_Betsapi : BaseJob {
	private const string JOB_NAME = nameof(FetchSoccerLiveMatchesJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<FetchSoccerLiveMatchesJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /1 * * * ?" :
				"0 /10 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	private readonly ILogger<FetchSoccerLiveMatchesJob_Betsapi> logger;
	private readonly BetsapiRepo betsapiRepo;

	public FetchSoccerLiveMatchesJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<FetchSoccerLiveMatchesJob_Betsapi> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.betsapiRepo = betsapiRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		// Onetime fetch
		var apiResult = await this.betsapiRepo.FetchInplayMatches<Betsapi_SoccerInplayMatchesData>(MstSportModelConst.Id_Soccer_betsapi);
		if (apiResult is null || apiResult.failed) {
			return;
		}

		var apiMatches = apiResult.results;

		foreach (var apiMatch in apiMatches) {
			var sysMatches = await this.dbContext.sportMatches.Where(m => m.ref_betsapi_match_id == apiMatch.id).ToArrayAsync();

			// Register new match with apiMatch info
			if (sysMatches.Length == 0) {
				sysMatches = new SportMatchModel[] { await this._RegisterNewMatch(MstSportModelConst.Id_Soccer, apiMatch) };
			}

			// Update matches info
			foreach (var sysMatch in sysMatches) {
				// Current play time
				var timer = apiMatch.timer;
				if (timer != null) {
					sysMatch.timer = (short)(timer.tm * 60 + timer.ts);
					sysMatch.timer_break = timer.tt == "1";
					sysMatch.timer_injury = (short)(timer.ta * 60);
				}

				// Update total timer.
				// Check esport: Esoccer Battle - 8 mins play
				var leagueName = apiMatch.league.name;
				if (leagueName != null && leagueName.StartsWithDk("Esoccer")) {
					var arr = apiMatch.league.name.Split('-');
					if (arr.Length >= 2) {
						arr = arr.Last().Trim().Split(' ');
						if (arr.Length > 0) {
							sysMatch.is_esport = true;
							sysMatch.total_timer = (short)(arr[0].Trim().ParseShortDk() * 60);
						}
					}
				}

				// Current scores
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

	private async Task<SportMatchModel> _RegisterNewMatch(int sport_id, Betsapi_SoccerInplayMatchesData.Result apiMatch) {
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
				name = apiMatch.league.name
			};
			this.dbContext.sportLeagues.Attach(targetLeague);
			await this.dbContext.SaveChangesAsync();
		}

		// Save home team and Get id
		if (homeTeam is null) {
			var image_id = await this.betsapiRepo.CalcImageId(apiMatch.home.image_id);

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
			var image_id = await this.betsapiRepo.CalcImageId(apiMatch.away.image_id);

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

			status = SportMatchModelConst.TimeStatus.InPlay,
			start_at = DateTimeOffset.FromUnixTimeSeconds(apiMatch.time).UtcDateTime,

			ref_betsapi_match_id = apiMatch.id,
			ref_betsapi_home_team_id = apiMatch.home.id,
			ref_betsapi_away_team_id = apiMatch.away.id,
		};

		this.dbContext.sportMatches.Attach(targetMatch);

		return targetMatch;
	}
}

[DisallowConcurrentExecution]
public class FetchSoccerUpcomingMatchesJob_Betsapi : BaseJob {
	private const string JOB_NAME = nameof(FetchSoccerUpcomingMatchesJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<FetchSoccerUpcomingMatchesJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 0 1 /1 * ?") // Every day
			.WithDescription(JOB_NAME)
		);
	}

	private readonly ILogger<FetchSoccerUpcomingMatchesJob_Betsapi> logger;
	private readonly BetsapiRepo betsapiRepo;

	public FetchSoccerUpcomingMatchesJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<FetchSoccerUpcomingMatchesJob_Betsapi> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.betsapiRepo = betsapiRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		var now = DateTime.UtcNow;
		var moreDay = 3;

		// Fetch next 3 days from today
		while (moreDay-- > 0) {
			// Padding 0: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
			var day = $"{(now.Year):D4}{(now.Month):D2}{(now.Day):D2}";

			// Recursive fetch
			await this._RecursiveFetchUpcomingMatches(day: day, page: 1);

			now = now.AddDays(1);
		}
	}

	private async Task _RecursiveFetchUpcomingMatches(string day, int page) {
		var apiResult = await this.betsapiRepo.FetchUpcomingMatches<Betsapi_UpcomingMatchesData>(MstSportModelConst.Id_Soccer_betsapi, day, page);
		if (apiResult is null || apiResult.failed) {
			return;
		}

		var apiMatches = apiResult.results;

		foreach (var apiMatch in apiMatches) {
			var sysMatches = await this.dbContext.sportMatches.Where(m => m.ref_betsapi_match_id == apiMatch.id).ToArrayAsync();

			// Register new match with its info (league, team,...)
			if (sysMatches.Length == 0) {
				await this._RegisterNewMatch(MstSportModelConst.Id_Soccer, apiMatch);
			}
		}

		// Save all matches
		await this.dbContext.SaveChangesAsync();

		// Continuous fetch until reach to final page !
		if (apiResult.pager.page * apiResult.pager.per_page < apiResult.pager.total) {
			await this._RecursiveFetchUpcomingMatches(day, apiResult.pager.page + 1);
		}
	}

	private async Task<SportMatchModel> _RegisterNewMatch(int sport_id, Betsapi_UpcomingMatchesData.Result apiMatch) {
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
				name = apiMatch.league.name
			};
			this.dbContext.sportLeagues.Attach(targetLeague);
			await this.dbContext.SaveChangesAsync();
		}

		// Save home team and Get id
		if (homeTeam is null) {
			var image_id = await this.betsapiRepo.CalcImageId(apiMatch.home.image_id);

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
			var image_id = await this.betsapiRepo.CalcImageId(apiMatch.away.image_id);

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

			status = SportMatchModelConst.TimeStatus.Upcoming,
			start_at = DateTimeOffset.FromUnixTimeSeconds(apiMatch.time).UtcDateTime,

			ref_betsapi_match_id = apiMatch.id,
			ref_betsapi_home_team_id = apiMatch.home.id,
			ref_betsapi_away_team_id = apiMatch.away.id,
		};

		this.dbContext.sportMatches.Attach(targetMatch);

		return targetMatch;
	}
}
