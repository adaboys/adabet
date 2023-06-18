namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class Betsapi_UpdateOdds_Live_Job : Betsapi_UpdateOddsJob<Betsapi_UpdateOdds_Live_Job> {
	private const string JOB_NAME = nameof(Betsapi_UpdateOdds_Live_Job);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		var cronExpression = appSetting.environment == AppSetting.ENV_PRODUCTION ?
			"0 /1 * * * ?" :
			"0 /3 * * * ?"
		;
		quartzConfig.ScheduleJob<Betsapi_UpdateOdds_Live_Job>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(cronExpression)
			.WithDescription(JOB_NAME)
		);
	}

	public Betsapi_UpdateOdds_Live_Job(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<Betsapi_UpdateOdds_Live_Job> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot, logger, betsapiRepo) {
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		// We prior to matches that has long time from previous updated
		var sysMatches = await this.dbContext.sportMatches
			.Where(m => m.status == SportMatchModelConst.TimeStatus.InPlay)
			.OrderBy(m => m.updated_at)
			.ToArrayAsync()
		;

		await this.OnetimeUpdateMatchOddsAsync(sysMatches);
	}
}
