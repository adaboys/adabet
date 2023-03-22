namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;

/// Update exchange rate from world to our system.
/// Coin API: Free 100 daily requests.
[DisallowConcurrentExecution]
public class UpdateExchangeRateJob : BaseJob {
	private const string JOB_NAME = nameof(UpdateExchangeRateJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		//todo comment for now to avoid waste
		// quartzConfig.ScheduleJob<UpdateExchangeRateJob>(trigger => trigger
		// 	.WithIdentity(JOB_NAME)
		// 	.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
		// 	.WithCronSchedule("0 0 0 /1 * ?") // Should fill from left -> right
		// 	.WithDescription(JOB_NAME)
		// );
	}

	private readonly ILogger<UpdateExchangeRateJob> logger;
	private readonly ExchangeRateRepo exchangeRateRepo;

	public UpdateExchangeRateJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UpdateExchangeRateJob> logger,
		ExchangeRateRepo exchangeRateRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.exchangeRateRepo = exchangeRateRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		var response = await this.exchangeRateRepo.Ada2Usd();
		if (response is null) {
			return;
		}

		var ada2usd_rate = (decimal)response.rate;
		var usd2ada_rate = 1m / ada2usd_rate;

		var ada2usd_model = await this.dbContext.exchangeRates.FirstAsync(m =>
			m.from_currency == MstExchangeRateModelConst.ADA &&
			m.to_currency == MstExchangeRateModelConst.USD
		);
		ada2usd_model.closing_rate = ada2usd_rate;

		var usd2ada_model = await this.dbContext.exchangeRates.FirstAsync(m =>
			m.from_currency == MstExchangeRateModelConst.USD &&
			m.to_currency == MstExchangeRateModelConst.ADA
		);
		usd2ada_model.closing_rate = usd2ada_rate;

		await this.dbContext.SaveChangesAsync();
	}
}
