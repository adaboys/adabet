namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

/// Hub:
/// To notify clients via Hub at anywhere inside the app, we should NOT use DI (dependency injection) directly
/// on the Hub, should use IHubContext<Hub> to get the Hub, so can invoke client's methods inside the Hub.
/// What is Hub: https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-7.0
/// Get Hub: https://learn.microsoft.com/en-us/aspnet/core/signalr/hubcontext?view=aspnetcore-7.0

/// Authorization:
/// By default, all clients can call Hub without authentication.
/// To protect Hub, we use Authorize attribute (annotation).
/// To call authorized route, each client must provide access_token when init connection.
/// Ref: https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-7.0
public class BaseHub<T> : Hub<T> where T : class {
	/// Get user_id (extracted from the accessToken) which be hold in claims of current user.
	protected Guid? userId {
		get {
			if (Guid.TryParse(this.Context.User?.FindFirst(AppConst.jwt.claim_type_user_id)?.Value, out var result)) {
				return result;
			}
			return null;
		}
	}
}
