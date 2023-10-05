namespace App;

using Quartz;

public static class QuartzConfig {
	/// Configure quartz and add to services.
	/// Ref: https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/aspnet-core-integration.html#installation
	public static void ConfigureQuartzDk(this IServiceCollection service, ConfigurationManager config, AppSetting appSetting) {
		// Add config of Quartz
		service.Configure<QuartzOptions>(config.GetSection(AppSetting.SECTION_QUARTZ));

		// Register Quartz to services
		service.AddQuartz(quartzConfig => {
			// Handy when part of cluster or you want to otherwise identify multiple schedulers
			quartzConfig.SchedulerId = "Scheduler-Core";

			// As of 3.3.2 this also injects scoped services (like EF DbContext) without problems
			quartzConfig.UseMicrosoftDependencyInjectionJobFactory();

			// These are the defaults
			quartzConfig.UseSimpleTypeLoader();
			quartzConfig.UseInMemoryStore();
			quartzConfig.UseDefaultThreadPool(poolOption => {
				poolOption.MaxConcurrency = 10;
			});

			RegisterJobs(quartzConfig, appSetting);
		});

		// Handle with ASP.NET Core server
		service.AddQuartzServer(option => {
			// Don't allow shutdown process until all jobs completed.
			option.WaitForJobsToComplete = true;
		});
	}

	/// Register all jobs here.
	/// Quickest way to create a job with single trigger is to use ScheduleJob (requires version 3.2)
	private static void RegisterJobs(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		UpdateExchangeRateJob.Register(quartzConfig);

		DecideUserBetResultJob.Register(quartzConfig);
		SendRewardToWinnerJob.Register(quartzConfig);
		// SubmitUserBetToCardanoJob.Register(quartzConfig);

		SendPredictionRewardJob.Register(quartzConfig);

		// Soccer
		FetchLiveMatches_ForSoccerJob_Betsapi.Register(quartzConfig, appSetting);
		FetchUpcomingMatches_ForSoccerJob_Betsapi.Register(quartzConfig, appSetting);

		UpdateSoccerOdds_LiveJob_Betsapi.Register(quartzConfig, appSetting);
		UpdateSoccerOdds_UpcomingJob_Betsapi.Register(quartzConfig, appSetting);

		UpdateMatchStatus_LiveSoccerJob_Betsapi.Register(quartzConfig, appSetting);
		UpdateSoccerMatchStatus_UpcomingJob_Betsapi.Register(quartzConfig, appSetting);
		UpdateSoccerMatchStatus_ComingSoonJob_Betsapi.Register(quartzConfig, appSetting);

		// Tennis
		FetchLiveMatches_TennisJob_Betsapi.Register(quartzConfig, appSetting);
		FetchUpcomingMatches_TennisJob_Betsapi.Register(quartzConfig, appSetting);

		UpdateTennisOdds_LiveJob_Betsapi.Register(quartzConfig, appSetting);
		UpdateTennisOdds_UpcomingJob_Betsapi.Register(quartzConfig, appSetting);

		UpdateTennisMatchStatus_LiveJob_Betsapi.Register(quartzConfig, appSetting);
		UpdateTennisMatchStatus_UpcomingJob_Betsapi.Register(quartzConfig, appSetting);
		UpdateTennisMatchStatus_TmpPendingJob_Betsapi.Register(quartzConfig, appSetting);

		// PingPong
		FetchMatches_Live_PingPongJob_Betsapi.Register(quartzConfig, appSetting);
		FetchMatches_Upcoming_PingPongJob_Betsapi.Register(quartzConfig, appSetting);

		UpdateOdds_Live_PingPongJob_Betsapi.Register(quartzConfig, appSetting);
		UpdateOdds_Upcoming_PingPongJob_Betsapi.Register(quartzConfig, appSetting);

		UpdateMatchStatus_LiveJob_PingPongBetsapi.Register(quartzConfig, appSetting);
		UpdateMatchStatus_UpcomingJob_PingPong_Betsapi.Register(quartzConfig, appSetting);
		UpdateMatchStatus_TmpPendingJob_PingPongBetsapi.Register(quartzConfig, appSetting);

		// Sync user wallet balance
		SyncUserWalletBalanceJob.Register(quartzConfig);

		// Transaction -> Swap
		SubmitSwapToChainJob.Register(quartzConfig);

		// Event
		DeliverEventGiftJob.Register(quartzConfig);
	}
}

//    0               /1              *               *                 *                ?                 *
// Seconds(0-59)  Minutes(0-59)  Hours(0-23)  Day-of-Month(?,1-31)  Month(1-12)  Day-of-Week(?,1-7)  Year(optional)

// Test cron expression: https://crontab.guru/
// Support for specifying both a day-of-week AND a day-of-month parameter is not implemented
