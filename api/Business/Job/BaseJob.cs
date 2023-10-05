namespace App;

using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Quartz;

/// T: child class
public abstract class BaseJob<T> : IJob where T : class {
	protected readonly AppDbContext dbContext;
	protected readonly AppSetting appSetting;
	protected readonly ILogger<T> logger; // Type is required, otherwise get exception.
	protected readonly MailComponent? mailComponent;

	public BaseJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<T> logger,
		MailComponent? mailComponent = null
	) {
		this.dbContext = dbContext;
		this.appSetting = snapshot.Value;
		this.logger = logger;
		this.mailComponent = mailComponent;
	}

	/// Implement from base (IJob)
	public async Task Execute(IJobExecutionContext context) {
		// Only report when cronjob is enabled
		if (this.appSetting.taskMode.enableCronJob) {
			try {
				await this.Run(context);
			}
			catch (Exception e) {
				var methodName = $"{typeof(T).FullName}.{e.TargetSite?.Name}";

				var nextException = e;
				var exceptionMsg = e.Message;
				while (nextException != null) {
					exceptionMsg += $", nextException: {nextException.Message}";
					nextException = nextException.InnerException;
				}

				this.logger.ErrorDk(this, $"[{this.appSetting.environment}] Exception ocurred at {methodName} with Message: {exceptionMsg}, Trace: {e.StackTrace}");

				// Only send mail at production
				if (this.mailComponent != null && this.appSetting.environment == AppSetting.ENV_PRODUCTION) {
					await this.mailComponent.ReportToMeAsync(
						$"[{this.appSetting.environment}] Exception ocurred at {methodName}",
						$"Message: {exceptionMsg}, Trace: {e.StackTrace}"
					);
				}
			}
		}
	}

	public abstract Task Run(IJobExecutionContext context);
}
