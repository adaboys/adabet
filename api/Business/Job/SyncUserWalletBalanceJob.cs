namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class SyncUserWalletBalanceJob : BaseJob<SyncUserWalletBalanceJob> {
	private const string JOB_NAME = nameof(SyncUserWalletBalanceJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<SyncUserWalletBalanceJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 /15 * * * ?") // Each 15 minutes
			.WithDescription(JOB_NAME)
		);
	}

	private readonly CardanoNodeRepo cardanoNodeRepo;

	public SyncUserWalletBalanceJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<SyncUserWalletBalanceJob> logger,
		CardanoNodeRepo cardanoNodeRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger) {
		this.cardanoNodeRepo = cardanoNodeRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		var userWallets = await this.dbContext.userWallets.OrderBy(m => m.updated_at).Take(10).ToArrayAsync();
		if (userWallets.Length == 0) {
			return;
		}

		var coins = await this.dbContext.currencies.AsNoTracking().ToArrayAsync();

		// Use LinQ will deferrer (lazy) start task.
		// If we call ToArray() or ToList() on deferredTasks, it causes tasks be executed immediately.
		var deferredTasks = userWallets.Select(userWallet => _UpdateUserWalletBalanceAsync(userWallet, coins));

		await Task.WhenAll(deferredTasks);

		await this.dbContext.SaveChangesAsync();
	}

	private async Task _UpdateUserWalletBalanceAsync(UserWalletModel userWallet, MstCurrencyModel[] coins) {
		var balanceResponse = await this.cardanoNodeRepo.GetMergedAssetsAsync(userWallet.wallet_address);
		if (balanceResponse.failed) {
			return;
		}

		var balance = new List<WalletBalance>();
		foreach (var coin in coins) {
			var amount = CardanoHelper.CalcCoinAmountFromAssets(coin, balanceResponse.data.assets);
			balance.Add(new() { coin_id = coin.id, amount = amount });
		}

		userWallet.balance = DkJsons.ToJson(balance);
	}
}
