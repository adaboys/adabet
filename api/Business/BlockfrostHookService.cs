namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public class BlockfrostHookService : BaseService {
	private readonly ILogger<UserService> logger;
	private readonly IHubContext<NotificationHub, INotificationHub> notiHub;
	private readonly CardanoNodeRepo cardanoNodeRepo;

	public BlockfrostHookService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UserService> logger,
		IHubContext<NotificationHub, INotificationHub> notiHub,
		CardanoNodeRepo cardanoNodeRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.notiHub = notiHub;
		this.cardanoNodeRepo = cardanoNodeRepo;
	}

	public async Task<ApiResponse> OnNewTransaction(OnNewTransactionRequestBody requestBody) {
		// this.logger.InfoDk(this, "HOOKED data: {@data}", requestBody);

		var outAddrs = new HashSet<string>();
		foreach (var payload in requestBody.payload) {
			foreach (var output in payload.outputs) {
				outAddrs.Add(output.address);
			}
		}

		if (outAddrs.Count > 0) {
			var targetUsers = await this.dbContext.userWallets.AsNoTracking()
				.Where(m => outAddrs.Contains(m.wallet_address))
				.Select(m => new { m.user_id, m.wallet_address })
				.ToArrayAsync()
			;

			if (targetUsers.Length > 0) {
				var coins = await this.dbContext.currencies.AsNoTracking().ToArrayAsync();

				foreach (var item in targetUsers) {
					var balanceResponse = await this.cardanoNodeRepo.GetMergedAssetsAsync(item.wallet_address);

					if (balanceResponse.succeed) {
						var assets = balanceResponse.data.assets;
						var balance = coins
							.Select(m => new OnBalanceChangedData { coin_id = m.id, amount = CardanoHelper.CalcCoinAmountFromAssets(m, assets) })
							.ToArray()
						;

						this.logger.InfoDk(this, "balance ----> {@data}", balance);

						await this.notiHub.Clients.User(item.user_id.ToStringWithoutHyphen()).OnBalanceChanged(balance);
					}
				}
			}
		}

		return new ApiSuccessResponse();
	}
}
