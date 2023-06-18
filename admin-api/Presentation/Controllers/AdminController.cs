namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tool.Compet.Core;

[ApiController, Route(Routes.api_prefix)]
public class AdminController : BaseController {
	private readonly AdminService service;

	public AdminController(AdminService service) {
		this.service = service;
	}

	/// <summary>
	/// Get profile of the admin.
	/// </summary>
	/// <response code="200"></response>
	[RequireAdminDk]
	[HttpGet, Route(Routes.admin_profile)]
	public async Task<ActionResult<ApiResponse>> GetAdminProfile() {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await this.service.GetAdminProfile(userId.Value);
	}
}
