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
public class UpdateMatchStatus_LiveSoccerJob_Betsapi : Base_UpdateSoccerMatchStatusJob_Betsapi<UpdateMatchStatus_LiveSoccerJob_Betsapi> {
	private const string JOB_NAME = nameof(UpdateMatchStatus_LiveSoccerJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateMatchStatus_LiveSoccerJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /1 * * * ?" : // Should at 30s
				"0 /5 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public UpdateMatchStatus_LiveSoccerJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateMatchStatus_LiveSoccerJob_Betsapi> logger,
		MailComponent mailComponent,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent, betsapiRepo: betsapiRepo) {
	}

	/// It consumes at most 30 requests.
	public override async Task Run(IJobExecutionContext context) {
		var query =
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
				"0 /1 * * * ?" : // Should at 30s
				"0 /5 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public UpdateSoccerMatchStatus_ComingSoonJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateSoccerMatchStatus_ComingSoonJob_Betsapi> logger,
		MailComponent mailComponent,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent, betsapiRepo: betsapiRepo) {
	}

	/// It consumes at most 10 requests.
	public override async Task Run(IJobExecutionContext context) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			// Filter soccer or esoccer since they are same id at betsapi
			where MstSportModelConst.FootbalIds.Contains(_league.sport_id)
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
public class UpdateSoccerMatchStatus_UpcomingJob_Betsapi : Base_UpdateSoccerMatchStatusJob_Betsapi<UpdateSoccerMatchStatus_UpcomingJob_Betsapi> {
	private const string JOB_NAME = nameof(UpdateSoccerMatchStatus_UpcomingJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateSoccerMatchStatus_UpcomingJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /2 * * * ?" : // Should at 30s
				"0 /5 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	public UpdateSoccerMatchStatus_UpcomingJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateSoccerMatchStatus_UpcomingJob_Betsapi> logger,
		MailComponent mailComponent,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent, betsapiRepo: betsapiRepo) {
	}

	/// It consumes at most 10 requests.
	public override async Task Run(IJobExecutionContext context) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			// Filter soccer or esoccer since they are same id at betsapi
			where MstSportModelConst.FootbalIds.Contains(_league.sport_id)
			where _match.status == SportMatchModelConst.TimeStatus.Upcoming

			orderby _match.start_at

			select new {
				_match
			}
		;
		var reqSysMatches = await query.Select(m => m._match).Take(100).ToArrayAsync();

		await this.UpdateMatchesInfoPerTenAsync(reqSysMatches);
	}
}

public abstract class Base_UpdateSoccerMatchStatusJob_Betsapi<T> : BaseJob<T> where T : class {
	protected readonly BetsapiRepo betsapiRepo;

	public Base_UpdateSoccerMatchStatusJob_Betsapi(
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
		var apiResult = await this.betsapiRepo.FetchMatchDetail<BetsapiData_SoccerMatchDetail>(reqMatchIds);

		if (apiResult is null || apiResult.failed) {
			this.logger.ErrorDk(this, "Update soccer match status failed, result: {@data}", apiResult);
			return;
		}

		var apiMatches = apiResult.results;
		var apiMatchId_2_apiMatch = apiMatches.DistinctBy(m => m.id).ToDictionary(m => m.id);

		foreach (var sysMatch in sysMatches) {
			// Remap betsapi match-id if it is different with betsapi
			if (!apiMatchId_2_apiMatch.ContainsKey(sysMatch.ref_betsapi_match_id)) {
				await BetsapiHelper.RemapMatchIdAsync(sysMatch: sysMatch, betsapiRepo: this.betsapiRepo, logger: logger, caller: this);
			}

			// After update ref match-id, we check mapping again from api-matches
			var apiMatch = apiMatchId_2_apiMatch.GetValueOrDefault(sysMatch.ref_betsapi_match_id);

			// If no mapping, we consider the match was remapped/removed at betsapi.
			// So just suspend/lock the sys-match for further betting.
			if (apiMatch is null) {
				this.logger.WarningDk(this, "Mark as removed sys-match {1} ({2}) since no mapping with api-match", sysMatch.id, sysMatch.ref_betsapi_match_id);
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
