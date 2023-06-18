namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

public interface INotificationHub {
	public Task OnBalanceChanged(string coin_name, decimal new_balance);
}

public class NotificationHub : BaseHub<INotificationHub> {
	// [Authorize]
	// public async Task RequestSomething(string coin_name, decimal new_balance) {
	// 	await this.Clients.Caller.ResponseSomething(coin_name, new_balance);
	// }
}
