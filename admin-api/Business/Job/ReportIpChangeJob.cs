namespace App;

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Http;

[DisallowConcurrentExecution]
public class ReportIpChangeJob : BaseJob {
	private const string JOB_NAME = nameof(ReportIpChangeJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig, AppSetting appSetting) {
		quartzConfig.ScheduleJob<ReportIpChangeJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 0 /12 * * ?") // Every 12 hours
			.WithDescription(JOB_NAME)
		);
	}

	private readonly ILogger<ReportIpChangeJob> logger;
	private readonly MailComponent mailComponent;

	public ReportIpChangeJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<ReportIpChangeJob> logger,
		MailComponent mailComponent
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.mailComponent = mailComponent;
	}

	private static string? curIpAddr = null;

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		var curIpAddr = ReportIpChangeJob.curIpAddr;
		var nextIpAddr = await GetIPAddressAsync();

		if (curIpAddr != nextIpAddr) {
			ReportIpChangeJob.curIpAddr = nextIpAddr;
			await this.mailComponent.SendAsync(
				"darkcompet@gmail.com",
				"Staging's ip changed",
				$"Ip was changed: {curIpAddr} -> {nextIpAddr}"
			);
		}
	}

	static async Task<string?> GetIPAddressAsync() {
		var httpClient = new DkHttpClient();
		var address = await httpClient.GetForString("http://checkip.dyndns.org");
		if (address is null) {
			return null;
		}

		int first = address.IndexOf("Address: ") + 9;
		int last = address.LastIndexOf("</body>");
		address = address.Substring(first, last - first);

		return address;
	}
}
