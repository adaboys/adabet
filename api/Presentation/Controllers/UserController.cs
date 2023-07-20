namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tool.Compet.Core;

[ApiController, Route(Routes.api_prefix)]
public class UserController : BaseController {
	private readonly UserService userService;

	public UserController(UserService userService) {
		this.userService = userService;
	}

	/// <summary>
	/// Attempt to register new user. After attempted, the client need confirm email, and
	/// send opt_code to server via register/confirm api.
	/// </summary>
	/// <param name="requestBody"></param>
	/// <response code="200"></response>
	[HttpPost, Route(Routes.user_register_attempt)]
	public async Task<ActionResult<ApiResponse>> AttemptRegisterUser([FromBody] AttemptRegisterUserRequestBody requestBody) {
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await this.userService.AttemptRegisterUser(requestBody);
	}

	/// <summary>
	/// Confirm user registeration. If succeed, new user will be registered into the system.
	/// </summary>
	/// <param name="requestBody">
	/// - otp_code: The code that server have sent to client's email.
	/// </param>
	/// <response code="200"></response>
	[HttpPost, Route(Routes.user_register_confirm)]
	public async Task<ActionResult<ApiResponse>> ConfirmRegisterUser([FromBody] ConfirmRegisterUserRequestBody requestBody) {
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await this.userService.ConfirmRegisterUser(requestBody);
	}

	/// <summary>
	/// Change password of the user.
	/// </summary>
	/// <param name="requestBody"></param>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.user_password_change)]
	public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequestBody requestBody) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await this.userService.ChangePassword(userId.Value, requestBody, RequestHelper.GetClientIp(this.Request), RequestHelper.GetClientIp(this.Request));
	}

	/// <summary>
	/// Get profile of the user. Note: to get user's balance (ADA, NFT,...), call user/balance api.
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpGet, Route(Routes.user_profile)]
	public async Task<ActionResult<ApiResponse>> GetUserProfile() {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await this.userService.GetUserProfile(userId.Value);
	}

	/// <summary>
	/// Get balance (ADA, NFTs,...) of the user.
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpGet, Route(Routes.user_balance)]
	public async Task<ActionResult<ApiResponse>> GetUserBalance() {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await this.userService.GetUserBalance(userId.Value);
	}

	/// <summary>
	/// Update user's identity for KYC.
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.user_identity_update)]
	public async Task<ActionResult<ApiResponse>> UpdateUserIdentity([FromBody] UpdateUserIdentityRequestBody requestBody) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await this.userService.UpdateUserIdentity(userId.Value, requestBody);
	}

	/// <summary>
	/// Update user's identity for KYC.
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.user_avatar_update)]
	public async Task<ActionResult<ApiResponse>> UpdateAvatar([FromForm] UpdateUserAvatarRequestBody requestBody) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await this.userService.UpdateAvatar(userId.Value, requestBody);
	}

	/// <summary>
	/// Get user bet statistics.
	/// </summary>
	/// <param name="time">One of: today, week, month, all</param>
	/// <response code="200"></response>
	[Authorize]
	[HttpGet, Route(Routes.sport_user_bet_statistics)]
	public async Task<ActionResult<ApiResponse>> GetBetStatistics([FromQuery] string time) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}

		return await this.userService.GetBetStatistics(userId.Value, time);
	}
}
