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

	public BlockfrostHookService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UserService> logger,
		IHubContext<NotificationHub, INotificationHub> notiHub
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.notiHub = notiHub;
	}

	public async Task<ApiResponse> OnNewTransaction(OnNewTransactionRequestBody requestBody) {
		this.logger.InfoDk(this, "HOOKED data: {@data}", requestBody);

		// var outAddrs = new HashSet<string>();
		// foreach (var payload in requestBody.payload) {
		// 	foreach (var output in payload.outputs) {
		// 		outAddrs.Add(output.address);
		// 	}
		// }

		// if (outAddrs.Count > 0) {
		// 	var targetUsers = await this.dbContext.userWallets
		// 		.Where(m => outAddrs.Contains(m.wallet_address))
		// 		.Select(m => new { m.user_id, m.wallet_address })
		// 		.ToArrayAsync();

		// 	foreach (var item in targetUsers) {
		// 		await this.notiHub.Clients.User(item.user_id.ToStringDk()).OnBalanceChanged("todo", 100);
		// 	}
		// }

		return new ApiSuccessResponse();
	}
}
