namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class Betsapi_FetchLiveMatchesJob : BaseJob {
	private const string JOB_NAME = nameof(Betsapi_FetchLiveMatchesJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<Betsapi_FetchLiveMatchesJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 /3 * * * ?") // 1 api in 30s
			.WithDescription(JOB_NAME)
		);
	}

	private readonly ILogger<Betsapi_FetchLiveMatchesJob> logger;
	private readonly BetsapiRepo betsapiRepo;

	public Betsapi_FetchLiveMatchesJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<Betsapi_FetchLiveMatchesJob> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.betsapiRepo = betsapiRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		var sport_id = MstSportModelConst.Id_Football;

		var apiResult = await this.betsapiRepo.FetchInplayMatches(sport_id);
		if (apiResult is null || apiResult.failed) {
			return;
		}

		var apiMatches = apiResult.results;

		foreach (var apiMatch in apiMatches) {
			var sysMatches = await this.dbContext.sportMatches.Where(m => m.ref_betsapi_match_id == apiMatch.id).ToArrayAsync();

			// Register new match with its info (league, team,...)
			if (sysMatches.Length == 0) {
				sysMatches = new SportMatchModel[] { await this._RegisterNewMatch(sport_id, apiMatch) };
			}

			// Update matches info
			foreach (var sysMatch in sysMatches) {
				// Current play time
				var timer = apiMatch.timer;
				if (timer != null) {
					sysMatch.timer = (short)(timer.tm * 60 + timer.ts);
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

	private async Task<SportMatchModel> _RegisterNewMatch(int sport_id, Betsapi_InplayMatchesData.Result apiMatch) {
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
