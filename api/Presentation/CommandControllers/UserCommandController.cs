namespace App;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

/// Example to run BootProject api at local:
/// httprepl https://localhost:8000
/// cd api/BootProject
/// post
[ApiController, Route(Routes.api_prefix)]
public class UserCommandController : ControllerBase {
	private readonly AppSetting appSetting;
	private readonly ILogger logger;
	private readonly UserCommand service;

	public UserCommandController(
		IOptionsSnapshot<AppSetting> option,
		ILogger<UserCommandController> logger,
		UserCommand service
	) {
		this.appSetting = option.Value;
		this.logger = logger;
		this.service = service;
	}

	/// <summary>
	/// Update codes for users.
	/// </summary>
	/// <response code="200"></response>
	[HttpPost, Route("cmd/users/update_codes")]
	public async Task<ApiResponse> UpdateCodesForUsers() {
		if (!this.appSetting.taskMode.enableCommand) {
			return new ApiBadRequestResponse("Bad mode");
		}
		return await this.service.UpdateCodesForUsers();
	}
}
