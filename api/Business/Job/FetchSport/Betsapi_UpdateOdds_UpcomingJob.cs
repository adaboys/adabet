namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class Betsapi_UpdateOdds_Upcoming_Job : Betsapi_UpdateOddsJob<Betsapi_UpdateOdds_Upcoming_Job> {
	private const string JOB_NAME = nameof(Betsapi_UpdateOdds_Upcoming_Job);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<Betsapi_UpdateOdds_Upcoming_Job>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 /5 * * * ?") // 10 api in 10s
			.WithDescription(JOB_NAME)
		);
	}

	public Betsapi_UpdateOdds_Upcoming_Job(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<Betsapi_UpdateOdds_Upcoming_Job> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot, logger, betsapiRepo) {
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		// １０試合ずつを順次に更新していく
		var sysMatches = await this.dbContext.sportMatches
			.Where(m => m.status == SportMatchModelConst.TimeStatus.Upcoming)
			.OrderBy(m => m.updated_at)
			.Take(10)
			.ToArrayAsync()
		;

		await this.UpdateMatchOddsAsync(sysMatches);
	}
}
