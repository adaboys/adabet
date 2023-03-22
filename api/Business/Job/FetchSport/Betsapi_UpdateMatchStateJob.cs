namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;
using Tool.Compet.Log;

[DisallowConcurrentExecution]
public class Betsapi_UpdateMatchStateJob : BaseJob {
	private const string JOB_NAME = nameof(Betsapi_UpdateMatchStateJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<Betsapi_UpdateMatchStateJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("/10 * * * * ?") // Run at every 10s. Upto 150 req (100 live + 10 upcoming + 10 pausing).
			.WithDescription(JOB_NAME)
		);
	}

	private readonly ILogger<Betsapi_UpdateMatchStateJob> logger;
	private readonly BetsapiRepo betsapiRepo;

	public Betsapi_UpdateMatchStateJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<Betsapi_UpdateMatchStateJob> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.betsapiRepo = betsapiRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		var sysLiveMatches = await this.dbContext.sportMatches
			.Where(m => m.status == SportMatchModelConst.TimeStatus.InPlay)
			.OrderBy(m => m.start_at)
			.ToArrayAsync()
		;
		var sysUpcomingMatches = await this.dbContext.sportMatches
			.Where(m => m.status == SportMatchModelConst.TimeStatus.Upcoming)
			.OrderBy(m => m.start_at)
			.Take(Math.Max(1, sysLiveMatches.Length / 10))
			.ToArrayAsync()
		;
		var sysPausingMatches = await this.dbContext.sportMatches
			.Where(m => SportMatchModelConst.PausingMatchStatusList.Contains(m.status))
			.OrderBy(m => m.start_at)
			.Take(Math.Max(1, sysLiveMatches.Length / 10))
			.ToArrayAsync()
		;

		var reqSysMatches = new List<SportMatchModel>(sysLiveMatches.Length + sysUpcomingMatches.Length + sysPausingMatches.Length);
		reqSysMatches.AddRange(sysLiveMatches);
		reqSysMatches.AddRange(sysUpcomingMatches);
		reqSysMatches.AddRange(sysPausingMatches);

		var apiMatchId_2_sysMatch = reqSysMatches.ToDictionary(m => m.ref_betsapi_match_id);

		// As betsapi's limitation, we can query at most 10 matches each time.
		foreach (var chunkMatches in reqSysMatches.Chunk(10)) {
			var reqMatchIds = chunkMatches.Select(m => m.ref_betsapi_match_id).ToArray();
			var apiResult = await this.betsapiRepo.FetchMatchDetail(reqMatchIds);

			if (apiResult is null || apiResult.failed) {
				return;
			}

			// Since duplication problem of matches in betsapi system,
			// an api match_id maybe does not match with our requested match_id.
			// To resolve the problem, we apply 2 rules to find mapping of them:
			// 1. Check matchness of home_team_id and away_team_id, if it does match, we consider they are same matches.
			// 2. Find in our db the match that mapping with api-match via `ref_betsapi_match_id` field.
			var apiMatchIds_notFoundInRequestedMatches = new HashSet<long>();
			foreach (var apiMatch in apiResult.results) {
				if (!apiMatchId_2_sysMatch.ContainsKey(apiMatch.id)) {
					apiMatchIds_notFoundInRequestedMatches.Add(apiMatch.id);
				}
			}
			var apiMatches_notFoundInOurSystem = new Dictionary<long, SportMatchModel>();
			if (apiMatchIds_notFoundInRequestedMatches.Count > 0) {
				apiMatches_notFoundInOurSystem = await this.dbContext.sportMatches
					.Where(m => apiMatchIds_notFoundInRequestedMatches.Contains(m.ref_betsapi_match_id))
					.ToDictionaryAsync(m => m.ref_betsapi_match_id);
			}

			var sysMatchIds_foundInBetsapi = new HashSet<long>();
			foreach (var apiMatch in apiResult.results) {
				// Find 1-1 mapping between our match_id vs api match_id.
				var sysMatch = apiMatchId_2_sysMatch.GetValueOrDefault(apiMatch.id);

				// Find from our requested matches.
				if (sysMatch is null) {
					sysMatch = reqSysMatches.FirstOrDefault(m => m.ref_betsapi_home_team_id == apiMatch.home.id && m.ref_betsapi_away_team_id == apiMatch.away.id);
				}
				// Find from db
				if (sysMatch is null) {
					sysMatch = apiMatches_notFoundInOurSystem.GetValueOrDefault(apiMatch.id);
				}
				// Hmm, not found (maybe call api merge_history to find)
				// For match that not found in our system, we disable the sys-match for betting later.
				if (sysMatch is null) {
					this.logger.WarningDk(this, "Not found sys-match that maps with api-match ({match_id}: {home_id} vs {away_id})", apiMatch.id, apiMatch.home.id, apiMatch.away.id);
					continue;
				}

				// OK, found the sys-match.
				// We remember it to disable not-found matches.
				sysMatchIds_foundInBetsapi.Add(sysMatch.id);

				// Update status
				sysMatch.status = SportMatchModelConst.ConvertFromBetsapiStatus(apiMatch.time_status);

				// Update play time
				var timer = apiMatch.timer;
				if (timer != null) {
					sysMatch.cur_play_time = (short)(timer.tm * 60 + timer.ts);
				}

				// Update scores
				if (apiMatch.ss != null) {
					var scores = apiMatch.ss.Split('-');

					if (scores.Length == 2) {
						sysMatch.home_score = scores[0].ParseShortDk();
						sysMatch.away_score = scores[1].ParseShortDk();
					}
				}
			}

			// Disable for betting
			foreach (var sysMatch in chunkMatches) {
				if (!sysMatchIds_foundInBetsapi.Contains(sysMatch.id)) {
				}
			}
		}

		// Save all matches
		await this.dbContext.SaveChangesAsync();
	}
}
