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
public class UpdateSoccerMatchStatus_LiveJob_Betsapi : Base_UpdateSoccerMatchStatusJob_Betsapi<UpdateSoccerMatchStatus_LiveJob_Betsapi> {
	private const string JOB_NAME = nameof(UpdateSoccerMatchStatus_LiveJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateSoccerMatchStatus_LiveJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /1 * * * ?" : // Should run at every minute
				"0 /10 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public UpdateSoccerMatchStatus_LiveJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateSoccerMatchStatus_LiveJob_Betsapi> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot, logger, betsapiRepo) {
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		// 10 call for 100 matches
		var query =
			from _match in this.dbContext.sportMatches
			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			where _league.sport_id == MstSportModelConst.Id_Soccer
			where _match.status == SportMatchModelConst.TimeStatus.InPlay
			orderby _match.start_at, _match.updated_at
			select new {
				_match
			}
		;
		var reqSysMatches = await query.Select(m => m._match).Take(300).ToArrayAsync();

		await this.UpdateMatchesInfoPerTenAsync(reqSysMatches);
	}
}

[DisallowConcurrentExecution]
public class UpdateSoccerMatchStatus_ComingSoonJob_Betsapi : Base_UpdateSoccerMatchStatusJob_Betsapi<UpdateSoccerMatchStatus_ComingSoonJob_Betsapi> {
	private const string JOB_NAME = nameof(UpdateSoccerMatchStatus_ComingSoonJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateSoccerMatchStatus_ComingSoonJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /2 * * * ?" : // Should every minute
				"0 /10 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public UpdateSoccerMatchStatus_ComingSoonJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateSoccerMatchStatus_ComingSoonJob_Betsapi> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot, logger, betsapiRepo) {
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		// 10 call for 100 matches
		var query =
			from _match in this.dbContext.sportMatches
			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			where _league.sport_id == MstSportModelConst.Id_Soccer
			where SportMatchModelConst.ComingSoonMatchStatusList.Contains(_match.status)
			orderby _match.start_at
			select new {
				_match
			}
		;
		var comingSoonMatches = await query.Select(m => m._match).Take(100).ToArrayAsync();

		await this.UpdateMatchesInfoPerTenAsync(comingSoonMatches);
	}
}

[DisallowConcurrentExecution]
public class UpdateSoccerMatchStatus_UpcomingJob_Betsapi : BaseJob {
	private const string JOB_NAME = nameof(UpdateSoccerMatchStatus_UpcomingJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateSoccerMatchStatus_UpcomingJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /5 * * * ?" : // Should run at every minute
				"0 /10 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	private readonly ILogger<UpdateSoccerMatchStatus_UpcomingJob_Betsapi> logger;
	private readonly BetsapiRepo betsapiRepo;

	public UpdateSoccerMatchStatus_UpcomingJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateSoccerMatchStatus_UpcomingJob_Betsapi> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.betsapiRepo = betsapiRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		// 10 call for 100 matches
		// 開始時間が先の試合のステータスを先に更新する
		var query =
			from _match in this.dbContext.sportMatches
			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			where _league.sport_id == MstSportModelConst.Id_Soccer
			where _match.status == SportMatchModelConst.TimeStatus.Upcoming
			orderby _match.start_at
			select new {
				_match
			}
		;
		var reqSysMatches = await query.Select(m => m._match).Take(100).ToArrayAsync();

		await this._UpdateMatchesInfoPerTenAsync(reqSysMatches);
	}

	protected async Task _UpdateMatchesInfoPerTenAsync(SportMatchModel[] reqSysMatches) {
		if (reqSysMatches.Count() == 0) {
			return;
		}

		// As betsapi's limitation, we can query at most 10 matches each time.
		// Use LinQ will deferrer (lazy) start task. If we call ToArray() or ToList() on deferredTasks,
		// it causes tasks be executed immediately.
		var deferredTasks = reqSysMatches.Chunk(10).Select(sysMatches => _UpdateMatchStatusAsync(sysMatches));

		// Run tasks parallel.
		// Each task maybe run at different thread, we need take care of concurency when interact with dbContext !
		await Task.WhenAll(deferredTasks);

		// Save changes
		await this.dbContext.SaveChangesAsync();
	}

	private async Task _UpdateMatchStatusAsync(SportMatchModel[] sysMatches) {
		// TechNote: Need convert to set before convert to array to avoid duplication.
		var reqMatchIds = sysMatches.Select(m => m.ref_betsapi_match_id).Distinct().ToArray();
		var apiResult = await this.betsapiRepo.FetchMatchDetail_Upcoming<BetsapiData_SoccerMatchDetail_Upcoming>(reqMatchIds);

		if (apiResult is null || apiResult.failed) {
			this.logger.ErrorDk(this, "API failed, result: {@data}", apiResult);
			return;
		}

		var apiMatches = apiResult.results;
		var apiMatchId_2_apiMatch = apiMatches.DistinctBy(m => m.id).ToDictionary(m => m.id);

		foreach (var sysMatch in sysMatches) {
			// Since match_merging specs in betsapi system, a returned match_id in betsapi maybe does not equals to our requested `ref_betsapi_match_id`.
			// If it happens, that is, `ref_betsapi_match_id` was merged to other api-match.
			// We just try to call api event/view and update `ref_betsapi_match_id` in sysMatch.
			if (!apiMatchId_2_apiMatch.ContainsKey(sysMatch.ref_betsapi_match_id)) {
				this.logger.InfoDk(this, "Called api event/view for sys-match {1}, {2}", sysMatch.id, sysMatch.ref_betsapi_match_id);

				var matchDetailResponse = await this.betsapiRepo.FetchMatchDetail_Upcoming<BetsapiData_SoccerMatchDetail_Upcoming>(sysMatch.ref_betsapi_match_id);
				if (matchDetailResponse is null || matchDetailResponse.failed) {
					this.logger.ErrorDk(this, "API detail failed: {@data}", matchDetailResponse);
					continue;
				}

				// Update ref_betsapi_match_id and lock betting + prediction to avoid take action on it.
				if (matchDetailResponse.results.Count > 0) {
					sysMatch.ref_betsapi_match_id = matchDetailResponse.results[0].id;
					sysMatch.lock_mode = SportMatchModelConst.LockMode.LockBetting;
				}
			}

			// After update ref match-id, we check mapping again from api-matches
			var apiMatch = apiMatchId_2_apiMatch.GetValueOrDefault(sysMatch.ref_betsapi_match_id);

			// Still no mapping -> the match was removed at betsapi -> suspend/lock the sys-match
			if (apiMatch is null) {
				this.logger.WarningDk(this, "Mark as removed sys-match {1}, {2} since no mapping with api-match", sysMatch.id, sysMatch.ref_betsapi_match_id);
				sysMatch.status = SportMatchModelConst.TimeStatus.RemovedSinceNotFoundInBetsapi;
				sysMatch.lock_mode = SportMatchModelConst.LockMode.LockBetting;
				continue;
			}

			// Update status
			sysMatch.status = SportMatchModelConst.ConvertStatusFromBetsapi(apiMatch.time_status);

			// Update start time
			sysMatch.start_at = DateTimeOffset.FromUnixTimeSeconds(apiMatch.time).UtcDateTime;

			// Update scores
			if (apiMatch.ss != null) {
				var scores = apiMatch.ss.Split('-');

				if (scores.Length == 2) {
					sysMatch.home_score = scores[0].ParseShortDk();
					sysMatch.away_score = scores[1].ParseShortDk();
				}
			}
		}
	}
}

public abstract class Base_UpdateSoccerMatchStatusJob_Betsapi<T> : BaseJob where T : class {
	protected readonly ILogger<T> logger;
	protected readonly BetsapiRepo betsapiRepo;

	public Base_UpdateSoccerMatchStatusJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<T> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.betsapiRepo = betsapiRepo;
	}

	protected async Task UpdateMatchesInfoPerTenAsync(SportMatchModel[] reqSysMatches) {
		if (reqSysMatches.Count() == 0) {
			return;
		}

		// TechNote: run and return new task. Don't make new Task to run a job.
		// - Task.Run() without async/await will be CPU-bound.
		// - Call Task.AwaitAll() will block current thread. Use await Task.WhenAll() instead to make it become IO non blocking.

		// As betsapi's limitation, we can query at most 10 matches each time.
		// Use LinQ will deferrer (lazy) start task. If we call ToArray() or ToList() on deferredTasks,
		// it causes tasks be executed immediately.
		// Ref: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios
		var deferredTasks = reqSysMatches.Chunk(10).Select(m => _UpdateMatchStatusAsync(m));

		// Run tasks parallel.
		// Each task maybe run at different thread, we need take care of concurency when interact with dbContext !
		await Task.WhenAll(deferredTasks);

		// Save changes
		await this.dbContext.SaveChangesAsync();
	}

	private async Task _UpdateMatchStatusAsync(SportMatchModel[] sysMatches) {
		// Take distinct since matches in system maybe match with a match in betsapi.
		var reqMatchIds = sysMatches.Select(m => m.ref_betsapi_match_id).Distinct().ToArray();
		var apiResult = await this.betsapiRepo.FetchMatchDetail<BetsapiData_SoccerMatchDetail>(reqMatchIds);

		if (apiResult is null || apiResult.failed) {
			this.logger.ErrorDk(this, "API failed, result: {@data}", apiResult);
			return;
		}

		var apiMatches = apiResult.results;
		var apiMatchId_2_apiMatch = apiMatches.DistinctBy(m => m.id).ToDictionary(m => m.id);

		foreach (var sysMatch in sysMatches) {
			// Since match_merging specs in betsapi system, a returned match_id in betsapi maybe does not equals to our requested `ref_betsapi_match_id`.
			// If it happens, that is, `ref_betsapi_match_id` was merged to other api-match.
			// We just try to call api event/view and update `ref_betsapi_match_id` in sysMatch.
			if (!apiMatchId_2_apiMatch.ContainsKey(sysMatch.ref_betsapi_match_id)) {
				this.logger.InfoDk(this, "Called api event/view for sys-match {1}, {2}", sysMatch.id, sysMatch.ref_betsapi_match_id);

				var matchDetailResponse = await this.betsapiRepo.FetchMatchDetail<BetsapiData_SoccerMatchDetail>(sysMatch.ref_betsapi_match_id);
				if (matchDetailResponse is null || matchDetailResponse.failed) {
					this.logger.ErrorDk(this, "API failed: {@data}", matchDetailResponse);
					continue;
				}

				// Update ref_betsapi_match_id and lock betting + prediction to avoid take action on it.
				if (matchDetailResponse.results.Count > 0) {
					sysMatch.ref_betsapi_match_id = matchDetailResponse.results[0].id;
					sysMatch.lock_mode = SportMatchModelConst.LockMode.LockBetting;
				}
			}

			// After update ref match-id, we check mapping again from api-matches
			var apiMatch = apiMatchId_2_apiMatch.GetValueOrDefault(sysMatch.ref_betsapi_match_id);

			// Still no mapping -> the match was removed at betsapi -> suspend/lock the sys-match
			if (apiMatch is null) {
				this.logger.WarningDk(this, "Mark as removed sys-match {1}, {2} since no mapping with api-match", sysMatch.id, sysMatch.ref_betsapi_match_id);
				sysMatch.status = SportMatchModelConst.TimeStatus.RemovedSinceNotFoundInBetsapi;
				sysMatch.lock_mode = SportMatchModelConst.LockMode.LockBetting;
				continue;
			}

			// Update status
			sysMatch.status = SportMatchModelConst.ConvertStatusFromBetsapi(apiMatch.time_status);

			// Update timer
			var timer = apiMatch.timer;
			if (timer != null) {
				sysMatch.timer = (short)(timer.tm * 60 + timer.ts);
				sysMatch.timer_break = timer.tt == "1";
				sysMatch.timer_injury = (short)(timer.ta * 60);
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
	}
}
