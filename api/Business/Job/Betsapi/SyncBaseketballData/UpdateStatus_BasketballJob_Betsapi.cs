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
public class UpdateStatus_BasketballJob_Betsapi : BaseJob<UpdateStatus_BasketballJob_Betsapi> {
	private const string JOB_NAME = nameof(UpdateStatus_BasketballJob_Betsapi);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<UpdateStatus_BasketballJob_Betsapi>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(appSetting.environment == AppSetting.ENV_PRODUCTION ?
				"0 /1 * * * ?" : // Should at 30s
				"0 /5 * * * ?"
			)
			.WithDescription(JOB_NAME)
		);
	}

	protected readonly BetsapiRepo betsapiRepo;

	public UpdateStatus_BasketballJob_Betsapi(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateStatus_BasketballJob_Betsapi> logger,
		MailComponent mailComponent,
		BetsapiRepo betsapiRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent) {
		this.betsapiRepo = betsapiRepo;
	}

	/// It consumes at most 30 requests.
	public override async Task Run(IJobExecutionContext context) {
		// Get live matches
		var queryLiveMatches =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			where _league.sport_id == MstSportModelConst.Id_Basketball
			where _match.status == SportMatchModelConst.TimeStatus.InPlay

			// We prior to matches that has long time from previous updated
			orderby _match.updated_at

			select new {
				_match
			}
		;
		var liveMatches = await queryLiveMatches.Select(m => m._match).Take(300).ToArrayAsync();

		// Get coming soon matches
		var queryPendingMatches =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			where _league.sport_id == MstSportModelConst.Id_Basketball
			where SportMatchModelConst.TmpPendingMatchStatusList.Contains(_match.status)

			// We prior to matches that has long time from previous updated
			orderby _match.updated_at

			select new {
				_match
			}
		;
		var pendingMatches = await queryPendingMatches.Select(m => m._match).Take(100).ToArrayAsync();

		// Get upcoming matches
		var queryUpcomingMatches =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			where _league.sport_id == MstSportModelConst.Id_Basketball
			where _match.status == SportMatchModelConst.TimeStatus.Upcoming

			// 開始時間が先の試合のステータスを先に更新する
			orderby _match.start_at

			select new {
				_match
			}
		;
		var upcomingMatches = await queryUpcomingMatches.Select(m => m._match).Take(100).ToArrayAsync();

		// Merge matches and sync
		var sysMatches = new List<SportMatchModel>(100);
		sysMatches.AddRange(liveMatches);
		sysMatches.AddRange(pendingMatches);
		sysMatches.AddRange(upcomingMatches);

		await this._UpdateMatchesInfoPerTenAsync(sysMatches);
	}

	protected async Task _UpdateMatchesInfoPerTenAsync(List<SportMatchModel> sysMatches) {
		if (sysMatches.Count == 0) {
			return;
		}

		// TechNote: run and return new task. Don't make new Task to run a job.
		// - Task.Run() without async/await will be CPU-bound.
		// - Call Task.AwaitAll() will block current thread. Use await Task.WhenAll() instead to make it become IO non blocking.

		// As betsapi's limitation, we can query at most 10 matches each time.
		// Use LinQ will deferrer (lazy) start task. If we call ToArray() or ToList() on deferredTasks,
		// it causes tasks be executed immediately.
		// Ref: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios
		var deferredTasks = sysMatches.Chunk(10).Select(m => _UpdateMatchStatusAsync(m));

		// Run tasks parallel.
		// Each task maybe run at different thread, we need take care of concurency when interact with dbContext !
		await Task.WhenAll(deferredTasks);

		// Save changes
		await this.dbContext.SaveChangesAsync();
	}

	private async Task _UpdateMatchStatusAsync(SportMatchModel[] sysMatches) {
		// Take distinct since matches in system maybe match with a match in betsapi.
		var reqMatchIds = sysMatches.Select(m => m.ref_betsapi_match_id).Distinct().ToArray();
		var apiResult = await this.betsapiRepo.FetchMatchDetail<BetsapiData_BasketballMatchDetail>(reqMatchIds);

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

			// Update timer
			sysMatch.timer = ((int)DateTimeOffset.FromUnixTimeSeconds(apiMatch.time).UtcDateTime.Subtract(sysMatch.start_at).TotalSeconds);

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
			// For Basketball, scores upToNow is same with ss.
			sysMatch.scores = apiMatch.ss;
		}
	}
}
