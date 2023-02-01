namespace App;

using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Quartz;

public abstract class BaseJob : IJob {
	protected readonly AppDbContext dbContext;
	protected readonly AppSetting appSetting;

	public BaseJob(AppDbContext dbContext, IOptionsSnapshot<AppSetting> snapshot) {
		this.dbContext = dbContext;
		this.appSetting = snapshot.Value;
	}

	public async Task Execute(IJobExecutionContext context) {
		if (this.appSetting.taskMode.enableCronJob) {
			await this.Run(context);
		}
	}

	public abstract Task Run(IJobExecutionContext context);
}
