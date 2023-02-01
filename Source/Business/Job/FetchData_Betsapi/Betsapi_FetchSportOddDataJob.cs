namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class Betsapi_FetchSportOddDataJob : BaseJob {
	private const string JOB_NAME = nameof(Betsapi_FetchSportOddDataJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<Betsapi_FetchSportOddDataJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 0 /1 * * ?") // Every 10 seconds
			.WithDescription(JOB_NAME)
		);
	}

	private readonly ILogger<Betsapi_FetchSportOddDataJob> logger;
	private readonly BetsapiRepo betsapiRepo;

	public Betsapi_FetchSportOddDataJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<Betsapi_FetchSportOddDataJob> logger,
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

		var targetMatches = await this.dbContext.sportMatches
			.Where(m => m.status == SportMatchModelConst.TimeStatus.InPlay || m.status == SportMatchModelConst.TimeStatus.NotYetStarted)
			.ToArrayAsync()
		;
		//todo fetch multiple matches by pass joined ids to api
		foreach (var match in targetMatches) {
			var apiResult = await this.betsapiRepo.FetchOddsSummary(sport_id, match.ref_betsapi_match_id);
			if (apiResult is null || apiResult.failed) {
				continue;
			}
			// apiResult.results._1XBet.odds.start._1_1.
		}

		// Save all matches
		await this.dbContext.SaveChangesAsync();

		//todo should notify all clients if some match data has changed??
		//todo should clear redis cache since we will cache query result at other apis?
	}
}
