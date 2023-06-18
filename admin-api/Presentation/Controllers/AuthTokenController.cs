namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tool.Compet.Core;

[ApiController, Route(Routes.api_prefix)]
public class AuthTokenController : BaseController {
	private readonly AuthTokenService authTokenService;
	private readonly ILogger<AuthTokenController> logger;

	public AuthTokenController(AuthTokenService authTokenService, ILogger<AuthTokenController> logger) {
		this.authTokenService = authTokenService;
		this.logger = logger;
	}

	// /// <summary>
	// /// Check expires-status of given access_token or refresh_token.
	// /// </summary>
	// /// <param name="access_token">Target access_token for expiry validation</param>
	// /// <response code="200">
	// /// - expires_in: Timeout in seconds of given access_token.
	// /// </response>
	// [HttpGet, Route(Routes.auth_token_validate)]
	// public async Task<ActionResult<ApiResponse>> ValidateAuthToken([FromQuery] string? access_token = null) {
	// 	return await authTokenService.ValidateAuthToken(access_token);
	// }
}
