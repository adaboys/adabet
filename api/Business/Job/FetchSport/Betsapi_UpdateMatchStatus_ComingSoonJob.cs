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
public class Betsapi_UpdateMatchStatus_ComingSoon_Job : Betsapi_UpdateMatchStatusJob<Betsapi_UpdateMatchStatus_ComingSoon_Job> {
	private const string JOB_NAME = nameof(Betsapi_UpdateMatchStatus_ComingSoon_Job);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		var cronExpression = appSetting.environment == AppSetting.ENV_PRODUCTION ?
			"0 /1 * * * ?" :
			"0 /3 * * * ?"
		;
		quartzConfig.ScheduleJob<Betsapi_UpdateMatchStatus_ComingSoon_Job>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule(cronExpression) // 10 api in 10s
			.WithDescription(JOB_NAME)
		);
	}

	public Betsapi_UpdateMatchStatus_ComingSoon_Job(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<Betsapi_UpdateMatchStatus_ComingSoon_Job> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot, logger, betsapiRepo) {
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		var comingSoonMatches = await this.dbContext.sportMatches
			.Where(m => SportMatchModelConst.ComingSoonMatchStatusList.Contains(m.status))
			.OrderBy(m => m.start_at)
			.Take(100)
			.ToArrayAsync()
		;

		await this.OnetimeUpdateMatchesInfoAsync(comingSoonMatches);
	}
}
