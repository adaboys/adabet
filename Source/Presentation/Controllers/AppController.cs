namespace App;

using Microsoft.AspNetCore.Mvc;

[ApiController, Route(Routes.api_prefix)]
public class AppController : BaseController {
	private readonly AppService service;

	public AppController(AppService service) {
		this.service = service;
	}

	/// <summary>
	/// Get app info.
	/// </summary>
	/// <param name="os_type">1 (android), 2 (ios), 3 (web), 4 (macos), 5 (windows)</param>
	/// <response code="200"></response>
	[HttpGet, Route(Routes.app_info)]
	public async Task<ActionResult<ApiResponse>> GetAppInfo([FromRoute] MstAppModelConst.OsType os_type) {
		return await service.GetAppInfo(os_type);
	}
}
