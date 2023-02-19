namespace App;

using Microsoft.EntityFrameworkCore;

public class AppService {
	private readonly AppDbContext dbContext;

	public AppService(AppDbContext dbContext) {
		this.dbContext = dbContext;
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
