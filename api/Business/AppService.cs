namespace App;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class AppService {
	private readonly AppDbContext dbContext;
	private readonly IHubContext<NotificationHub, INotificationHub> notiHub;


	public AppService(AppDbContext dbContext, IHubContext<NotificationHub, INotificationHub> notiHub) {
		this.dbContext = dbContext;
		this.notiHub = notiHub;
	}

	public async Task<ApiResponse> GetAppInfo(MstAppModelConst.OsType os_type) {
		var app = await dbContext.apps.Where(m => m.os_type == os_type).FirstOrDefaultAsync();
		if (app == null) {
			return new ApiNotFoundResponse();
		}

		return new AppResponse {
			data = new() {
				os_type = (byte)app.os_type,
				app_version = app.app_version,
			}
		};
	}
}
