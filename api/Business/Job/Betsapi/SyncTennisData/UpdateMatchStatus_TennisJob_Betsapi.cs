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
public class UpdateTennisMatchStatus_LiveJob_Betsapi : Base_UpdateTennisMatchStatusJob_Betsapi<UpdateTennisMatchStatus_LiveJob_Betsapi> {
	private const string JOB_NAME = nameof(UpdateTennisMatchStatus_LiveJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateTennisMatchStatus_LiveJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /1 * * * ?" : // Should at 30s
				"0 /5 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public UpdateTennisMatchStatus_LiveJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateTennisMatchStatus_LiveJob_Betsapi> logger,
		MailComponent mailComponent,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot, logger, mailComponent: mailComponent, betsapiRepo) {
	}

	/// It consumes at most 30 requests.
	public override async Task Run(IJobExecutionContext context) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			where _league.sport_id == MstSportModelConst.Id_Tennis
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
public class UpdateTennisMatchStatus_TmpPendingJob_Betsapi : Base_UpdateTennisMatchStatusJob_Betsapi<UpdateTennisMatchStatus_TmpPendingJob_Betsapi> {
	private const string JOB_NAME = nameof(UpdateTennisMatchStatus_TmpPendingJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateTennisMatchStatus_TmpPendingJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /1 * * * ?" : // Should at 30s
				"0 /5 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public UpdateTennisMatchStatus_TmpPendingJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateTennisMatchStatus_TmpPendingJob_Betsapi> logger,
		MailComponent mailComponent,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent, betsapiRepo: betsapiRepo) {
	}

	/// It consumes at most 10 requests.
	public override async Task Run(IJobExecutionContext context) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			where _league.sport_id == MstSportModelConst.Id_Tennis
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
public class UpdateTennisMatchStatus_UpcomingJob_Betsapi : Base_UpdateTennisMatchStatusJob_Betsapi<UpdateTennisMatchStatus_UpcomingJob_Betsapi> {
	private const string JOB_NAME = nameof(UpdateTennisMatchStatus_UpcomingJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateTennisMatchStatus_UpcomingJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /2 * * * ?" : // Should at 30s
				"0 /5 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public UpdateTennisMatchStatus_UpcomingJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateTennisMatchStatus_UpcomingJob_Betsapi> logger,
		MailComponent mailComponent,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent, betsapiRepo: betsapiRepo) {
	}

	/// It consumes at most 10 requests.
	public override async Task Run(IJobExecutionContext context) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			where _league.sport_id == MstSportModelConst.Id_Tennis
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

public abstract class Base_UpdateTennisMatchStatusJob_Betsapi<T> : BaseJob<T> where T : class {
	protected readonly BetsapiRepo betsapiRepo;

	public Base_UpdateTennisMatchStatusJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<T> logger,
		MailComponent mailComponent,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent) {
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
		var apiResult = await this.betsapiRepo.FetchMatchDetail<BetsapiData_TennisMatchDetail>(reqMatchIds);

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

			// Current point (point will make score when touch to 40 or Ace) in each set
			sysMatch.points = apiMatch.points;
			sysMatch.playing_indicator = apiMatch.playing_indicator;

			// Current score (last of ss), for eg,. "7-6,5-7,2-3"
			if (apiMatch.ss != null) {
				var scores = (apiMatch.ss ?? string.Empty).Split(',');

				if (scores.Length > 0) {
					var curScore = scores[scores.Length - 1].Split('-');

					if (curScore.Length == 2) {
						sysMatch.home_score = curScore[0].ParseShortDk();
						sysMatch.away_score = curScore[1].ParseShortDk();
					}
				}
			}

			// Scores up to now
			// For Tennis, scores upToNow is same with ss.
			sysMatch.scores = apiMatch.ss;
		}
	}
}
