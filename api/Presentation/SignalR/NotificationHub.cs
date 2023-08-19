namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

public interface INotificationHub {
	public Task<string> OnBalanceChanged(OnBalanceChangedData[] balance);
}

public class NotificationHub : BaseHub<INotificationHub> {
	[Authorize]
	public async Task OnBalanceChanged(OnBalanceChangedData[] balance) {
		await this.Clients.Caller.OnBalanceChanged(balance);
	}
}
