namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class Betsapi_FetchSportDataJob : BaseJob {
	private const string JOB_NAME = nameof(Betsapi_FetchSportDataJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<Betsapi_FetchSportDataJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 0 /1 * * ?") // Every 10 seconds
			.WithDescription(JOB_NAME)
		);
	}

	private readonly ILogger<Betsapi_FetchSportDataJob> logger;
	private readonly BetsapiRepo betsapiRepo;

	public Betsapi_FetchSportDataJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<Betsapi_FetchSportDataJob> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.betsapiRepo = betsapiRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		await this._UpsertFromBetsapiAsync();
	}

	private async Task _UpsertFromBetsapiAsync() {
		var sport_id = await this.dbContext.sports.Where(m => m.name == MstSportModelConst.Name_Football).Select(m => m.id).FirstAsync();
		var apiResult = await this.betsapiRepo.FetchInplayMatches(sport_id);
		if (apiResult is null) {
			return;
		}

		var apiMatches = apiResult.results;

		foreach (var apiMatch in apiMatches) {
			var targetMatch = await this.dbContext.sportMatches.FirstOrDefaultAsync(m =>
				m.ref_betsapi_home_team_id == apiMatch.home.id &&
				m.ref_betsapi_away_team_id == apiMatch.away.id
			);

			// Insert new match
			if (targetMatch is null) {
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
						sport_id = (await this.dbContext.sports.FirstAsync(m => m.name == MstSportModelConst.Name_Football)).id,
						name = apiMatch.league.name
					};
					this.dbContext.sportLeagues.Attach(targetLeague);
					await this.dbContext.SaveChangesAsync();
				}

				// Save home team and Get id
				if (homeTeam is null) {
					homeTeam = new() {
						name_native = apiMatch.home.name,
						name_en = apiMatch.home.name,
						//todo//fixme hardcode for now
						flag_image_relative_path = "todo_img_rpath",
						ref_betsapi_home_team_id = apiMatch.home.id
					};
					this.dbContext.sportTeams.Attach(homeTeam);
					await this.dbContext.SaveChangesAsync();
				}

				// Save away team and Get id
				if (awayTeam is null) {
					awayTeam = new() {
						name_native = apiMatch.away.name,
						name_en = apiMatch.away.name,
						//todo//fixme hardcode for now
						flag_image_relative_path = "todo_img_rpath",
						ref_betsapi_away_team_id = apiMatch.away.id
					};
					this.dbContext.sportTeams.Attach(awayTeam);
					await this.dbContext.SaveChangesAsync();
				}

				// Attach new match
				targetMatch = new() {
					league_id = targetLeague.id,
					home_team_id = homeTeam.id,
					away_team_id = awayTeam.id,

					cur_play_time = (short)(apiMatch.time.ParseLongDk() - DkDateTimes.currentUnixUtcTimeInMillis),

					status = SportMatchModelConst.TimeStatus.InPlay,
					start_at = DateTime.UtcNow,

					markets = new(),

					ref_betsapi_match_id = apiMatch.id,
					ref_betsapi_home_team_id = apiMatch.home.id,
					ref_betsapi_away_team_id = apiMatch.away.id,
				};

				this.dbContext.sportMatches.Attach(targetMatch);
			}

			// Update match info
			if (apiMatch.ss != null) {
				var scores = apiMatch.ss.Split('-');
				if (scores.Length == 2) {
					targetMatch.home_score = scores[0].ParseShortDk();
					targetMatch.away_score = scores[1].ParseShortDk();
				}
			}
		}

		// Save all matches
		await this.dbContext.SaveChangesAsync();

		//todo should notify all clients if some match data has changed??
		//todo should clear redis cache since we will cache query result at other apis?
	}
}
