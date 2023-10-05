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
public class UpdateMatchStatus_LiveJob_PingPongBetsapi : Base_UpdateMatchStatus_PingPongJob_Betsapi<UpdateMatchStatus_LiveJob_PingPongBetsapi> {
	private const string JOB_NAME = nameof(UpdateMatchStatus_LiveJob_PingPongBetsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateMatchStatus_LiveJob_PingPongBetsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /1 * * * ?" : // Should at 30s
				"0 /5 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public UpdateMatchStatus_LiveJob_PingPongBetsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateMatchStatus_LiveJob_PingPongBetsapi> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot, logger, betsapiRepo) {
	}

	/// It consumes at most 30 requests.
	public override async Task Run(IJobExecutionContext context) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			where _league.sport_id == MstSportModelConst.Id_PingPong
			where _match.status == SportMatchModelConst.TimeStatus.InPlay

			// We prior to matches that has long time from previous updated
			orderby _match.updated_at

			select new {
				_match
			}
		;
		var reqSysMatches = await query.Select(m => m._match).Take(300).ToArrayAsync();

		await this.UpdateMatchesInfoPerTenAsync(reqSysMatches);
	}
}

[DisallowConcurrentExecution]
public class UpdateMatchStatus_TmpPendingJob_PingPongBetsapi : Base_UpdateMatchStatus_PingPongJob_Betsapi<UpdateMatchStatus_TmpPendingJob_PingPongBetsapi> {
	private const string JOB_NAME = nameof(UpdateMatchStatus_TmpPendingJob_PingPongBetsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateMatchStatus_TmpPendingJob_PingPongBetsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /1 * * * ?" : // Should at 30s
				"0 /5 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public UpdateMatchStatus_TmpPendingJob_PingPongBetsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateMatchStatus_TmpPendingJob_PingPongBetsapi> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, betsapiRepo: betsapiRepo) {
	}

	/// It consumes at most 10 requests.
	public override async Task Run(IJobExecutionContext context) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			where _league.sport_id == MstSportModelConst.Id_PingPong
			where SportMatchModelConst.TmpPendingMatchStatusList.Contains(_match.status)

			// We prior to matches that has long time from previous updated
			orderby _match.updated_at

			select new {
				_match
			}
		;
		var comingSoonMatches = await query.Select(m => m._match).Take(100).ToArrayAsync();

		await this.UpdateMatchesInfoPerTenAsync(comingSoonMatches);
	}
}

[DisallowConcurrentExecution]
public class UpdateMatchStatus_UpcomingJob_PingPong_Betsapi : Base_UpdateMatchStatus_PingPongJob_Betsapi<UpdateMatchStatus_UpcomingJob_PingPong_Betsapi> {
	private const string JOB_NAME = nameof(UpdateMatchStatus_UpcomingJob_PingPong_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateMatchStatus_UpcomingJob_PingPong_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /2 * * * ?" : // Should at 30s
				"0 /5 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public UpdateMatchStatus_UpcomingJob_PingPong_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateMatchStatus_UpcomingJob_PingPong_Betsapi> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, betsapiRepo: betsapiRepo) {
	}

	/// It consumes at most 10 requests.
	public override async Task Run(IJobExecutionContext context) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			where _league.sport_id == MstSportModelConst.Id_PingPong
			where _match.status == SportMatchModelConst.TimeStatus.Upcoming

			// 開始時間が先の試合のステータスを先に更新する
			orderby _match.start_at

			select new {
				_match
			}
		;
		var reqSysMatches = await query.Select(m => m._match).Take(100).ToArrayAsync();

		await this.UpdateMatchesInfoPerTenAsync(reqSysMatches);
	}
}

public abstract class Base_UpdateMatchStatus_PingPongJob_Betsapi<T> : BaseJob<T> where T : class {
	protected readonly BetsapiRepo betsapiRepo;

	public Base_UpdateMatchStatus_PingPongJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<T> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger) {
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
		var apiResult = await this.betsapiRepo.FetchMatchDetail<BetsapiData_PingPongMatchDetail>(reqMatchIds);

		if (apiResult is null || apiResult.failed) {
			this.logger.ErrorDk(this, "API failed, result: {@data}", apiResult);
			return;
		}

		var apiMatches = apiResult.results;
		var apiMatchId_2_apiMatch = apiMatches.DistinctBy(m => m.id).ToDictionary(m => m.id);

		foreach (var sysMatch in sysMatches) {
			// Try remap match id
			if (!apiMatchId_2_apiMatch.ContainsKey(sysMatch.ref_betsapi_match_id)) {
				await BetsapiHelper.RemapMatchIdAsync(sysMatch: sysMatch, betsapiRepo: this.betsapiRepo, logger: logger, caller: this);
			}

			// After update ref match-id, we check mapping again from api-matches
			var apiMatch = apiMatchId_2_apiMatch.GetValueOrDefault(sysMatch.ref_betsapi_match_id);

			// If no mapping, we consider the match was remapped/removed at betsapi.
			// So just suspend/lock the sys-match for further betting.
			if (apiMatch is null) {
				this.logger.WarningDk(this, "Mark as removed sys-match {1}, {2} since no mapping with api-match", sysMatch.id, sysMatch.ref_betsapi_match_id);
				sysMatch.status = SportMatchModelConst.TimeStatus.RemovedSinceNotFoundInBetsapi;
				sysMatch.lock_mode = SportMatchModelConst.LockMode.LockBetting;
				continue;
			}

			// Update status
			sysMatch.status = SportMatchModelConst.ConvertStatusFromBetsapi(apiMatch.time_status);

			// Current score, for eg,. "7-6"
			if (apiMatch.ss != null) {
				// Current score
				var scores = (apiMatch.ss ?? string.Empty).Split(',');
				if (scores.Length > 0) {
					var curScores = scores[scores.Length - 1].Split('-');
					if (curScores.Length == 2) {
						sysMatch.home_score = curScores[0].ParseShortDk();
						sysMatch.away_score = curScores[1].ParseShortDk();
					}
				}
			}

			// Scores upToNow
			//fixme need convert to array or how to get more match 5, 6, 7, ...
			//for now, just use ss instead
			sysMatch.scores = apiMatch.ss;
			if (apiMatch.scores != null) {
				// sysMatch.scores = string.Empty +
				// 	apiMatch.scores._1.home + "-" + apiMatch.scores._1.away + "," +
				// 	apiMatch.scores._2.home + "-" + apiMatch.scores._2.away + "," +
				// 	apiMatch.scores._3.home + "-" + apiMatch.scores._3.away + "," +
				// 	apiMatch.scores._4.home + "-" + apiMatch.scores._4.away + ","
				// ;
			}
		}
	}
}
