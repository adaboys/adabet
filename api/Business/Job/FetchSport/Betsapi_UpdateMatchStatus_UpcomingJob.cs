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
public class Betsapi_UpdateMatchStatus_Upcoming_Job : BaseJob {
	private const string JOB_NAME = nameof(Betsapi_UpdateMatchStatus_Upcoming_Job);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		var cronExpression = appSetting.environment == AppSetting.ENV_PRODUCTION ? "0 /1 * * * ?" : "0 /3 * * * ?";

		quartzConfig.ScheduleJob<Betsapi_UpdateMatchStatus_Upcoming_Job>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(cronExpression)
			.WithDescription(JOB_NAME)
		);
	}

	private readonly ILogger<Betsapi_UpdateMatchStatus_Upcoming_Job> logger;
	private readonly BetsapiRepo betsapiRepo;

	public Betsapi_UpdateMatchStatus_Upcoming_Job(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<Betsapi_UpdateMatchStatus_Upcoming_Job> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.betsapiRepo = betsapiRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		// 開始時間が先の試合のステータスを先に更新する
		var reqSysMatches = await this.dbContext.sportMatches
			.Where(m => m.status == SportMatchModelConst.TimeStatus.Upcoming)
			.OrderBy(m => m.start_at)
			.Take(100)
			.ToArrayAsync()
		;

		await this._UpdateMatchesInfoAsync(reqSysMatches);
	}

	protected async Task _UpdateMatchesInfoAsync(SportMatchModel[] reqSysMatches) {
		if (reqSysMatches.Count() == 0) {
			return;
		}

		// As betsapi's limitation, we can query at most 10 matches each time.
		foreach (var sysMatches in reqSysMatches.Chunk(10)) {
			try {
				// TechNote: Need convert to set before convert to array to avoid duplication.
				var reqMatchIds = sysMatches.Select(m => m.ref_betsapi_match_id).Distinct().ToArray();
				var apiResult = await this.betsapiRepo.FetchMatchDetail_Upcoming(reqMatchIds);

				if (apiResult is null || apiResult.failed) {
					this.logger.ErrorDk(this, "API failed, result: {@data}", apiResult);
					return;
				}

				var apiMatches = apiResult.results;
				var resApiMatchId_2_apiMatch = apiMatches.DistinctBy(m => m.id).ToDictionary(m => m.id);

				// Since match_merging spec in betsapi system,
				// a returned match_id in betsapi maybe does not equals to our requested match_id.
				// To resolve it, we init `ref_betsapi_parent_match_id` before find mapping.
				foreach (var sysMatch in sysMatches) {
					// If no mapping from sys-match, that is, `ref_betsapi_match_id` was merged to other api-match.
					// Try to call api event/view to init `ref_betsapi_parent_match_id`
					if (!resApiMatchId_2_apiMatch.ContainsKey(sysMatch.ref_betsapi_match_id)) {
						this.logger.InfoDk(this, "Called api event/view for sys-match {1}, {2}", sysMatch.id, sysMatch.ref_betsapi_match_id);

						var matchDetailResponse = await this.betsapiRepo.FetchMatchDetail_Upcoming(sysMatch.ref_betsapi_match_id);
						if (matchDetailResponse is null || matchDetailResponse.failed) {
							this.logger.ErrorDk(this, "API detail failed: {@data}", matchDetailResponse);
							continue;
						}

						// Update ref api-match-id since the match is merged to parent match at betsapi.
						// Also lock betting, prediction to avoid take action on it.
						if (matchDetailResponse.results.Count > 0) {
							sysMatch.ref_betsapi_match_id = matchDetailResponse.results[0].id;
							sysMatch.lock_mode = SportMatchModelConst.LockMode.LockBetting;
						}
					}

					// After update ref match-id, we check mapping again from api-matches
					var mappingApiMatch = resApiMatchId_2_apiMatch.GetValueOrDefault(sysMatch.ref_betsapi_match_id);

					// Still no mapping -> the match was removed at betsapi -> suspend/lock the sys-match
					if (mappingApiMatch is null) {
						this.logger.WarningDk(this, "Mark as removed sys-match {1}, {2} since no mapping with api-match", sysMatch.id, sysMatch.ref_betsapi_match_id);
						sysMatch.status = SportMatchModelConst.TimeStatus.RemovedSinceNotFoundInBetsapi;
						sysMatch.lock_mode = SportMatchModelConst.LockMode.LockBetting;
						continue;
					}

					// Update status
					sysMatch.status = SportMatchModelConst.ConvertFromBetsapiStatus(mappingApiMatch.time_status);

					// Update start time
					sysMatch.start_at = DateTimeOffset.FromUnixTimeSeconds(mappingApiMatch.time).UtcDateTime;

					// Update scores
					if (mappingApiMatch.ss != null) {
						var scores = mappingApiMatch.ss.Split('-');

						if (scores.Length == 2) {
							sysMatch.home_score = scores[0].ParseShortDk();
							sysMatch.away_score = scores[1].ParseShortDk();
						}
					}
				}

				// Save changes of chunked matches
				await this.dbContext.SaveChangesAsync();
			}
			catch (Exception e) {
				this.logger.ErrorDk(this, "Could not update match status, error: {e}", e.Message);
			}
		}
	}
}
