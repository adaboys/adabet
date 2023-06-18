namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tool.Compet.Core;

[ApiController, Route(Routes.api_prefix)]
public class UserController : BaseController {
	private readonly UserService service;

	public UserController(UserService service) {
		this.service = service;
	}

	// /// <summary>
	// /// Attempt to register new user. After attempted, the client need confirm email, and
	// /// send opt_code to server via register/confirm api.
	// /// </summary>
	// /// <param name="requestBody"></param>
	// /// <response code="200"></response>
	// [HttpPost, Route(Routes.user_register_attempt)]
	// public async Task<ActionResult<ApiResponse>> AttemptRegisterUser([FromBody] AttemptRegisterUserRequestBody requestBody) {
	// 	DkReflections.TrimJsonAnnotatedProperties(requestBody);

	// 	return await this.service.AttemptRegisterUser(requestBody);
	// }

	// /// <summary>
	// /// Confirm user registeration. If succeed, new user will be registered into the system.
	// /// </summary>
	// /// <param name="requestBody">
	// /// - otp_code: The code that server have sent to client's email.
	// /// </param>
	// /// <response code="200"></response>
	// [HttpPost, Route(Routes.user_register_confirm)]
	// public async Task<ActionResult<ApiResponse>> ConfirmRegisterUser([FromBody] ConfirmRegisterUserRequestBody requestBody) {
	// 	DkReflections.TrimJsonAnnotatedProperties(requestBody);

	// 	return await this.service.ConfirmRegisterUser(requestBody);
	// }

	// /// <summary>
	// /// Change password of the user.
	// /// </summary>
	// /// <param name="requestBody"></param>
	// /// <response code="200"></response>
	// [Authorize]
	// [HttpPost, Route(Routes.user_password_change)]
	// public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequestBody requestBody) {
	// 	if (userId is null) {
	// 		return new ApiForbiddenResponse();
	// 	}
	// 	DkReflections.TrimJsonAnnotatedProperties(requestBody);

	// 	return await this.service.ChangePassword(userId.Value, requestBody, RequestHelper.GetClientIp(this.Request), RequestHelper.GetClientIp(this.Request));
	// }

	// /// <summary>
	// /// Get profile of the user. Note: to get user's balance (ADA, NFT,...), call user/balance api.
	// /// </summary>
	// /// <response code="200"></response>
	// [Authorize]
	// [HttpGet, Route(Routes.user_profile)]
	// public async Task<ActionResult<ApiResponse>> GetUserProfile() {
	// 	if (userId is null) {
	// 		return new ApiForbiddenResponse();
	// 	}
	// 	return await this.service.GetUserProfile(userId.Value);
	// }

	// /// <summary>
	// /// Get balance (ADA, NFTs,...) of the user.
	// /// </summary>
	// /// <response code="200"></response>
	// [Authorize]
	// [HttpGet, Route(Routes.user_balance)]
	// public async Task<ActionResult<ApiResponse>> GetUserBalance() {
	// 	if (userId is null) {
	// 		return new ApiForbiddenResponse();
	// 	}
	// 	return await this.service.GetUserBalance(userId.Value);
	// }

	// /// <summary>
	// /// Update user's identity for KYC.
	// /// </summary>
	// /// <response code="200"></response>
	// [Authorize]
	// [HttpPost, Route(Routes.user_identity_update)]
	// public async Task<ActionResult<ApiResponse>> UpdateUserIdentity([FromBody] UpdateUserIdentityRequestBody requestBody) {
	// 	if (userId is null) {
	// 		return new ApiForbiddenResponse();
	// 	}
	// 	DkReflections.TrimJsonAnnotatedProperties(requestBody);

	// 	return await this.service.UpdateUserIdentity(userId.Value, requestBody);
	// }

	// /// <summary>
	// /// Update user's identity for KYC.
	// /// </summary>
	// /// <response code="200"></response>
	// [Authorize]
	// [HttpPost, Route(Routes.user_avatar_update)]
	// public async Task<ActionResult<ApiResponse>> UpdateAvatar([FromForm] UpdateUserAvatarRequestBody requestBody) {
	// 	if (userId is null) {
	// 		return new ApiForbiddenResponse();
	// 	}
	// 	DkReflections.TrimJsonAnnotatedProperties(requestBody);

	// 	return await this.service.UpdateAvatar(userId.Value, requestBody);
	// }

	/// <summary>
	/// Get user list.
	/// </summary>
	/// <param name="sort_type">Sort by field_asc or field_desc. Where field is: name, created_at</param>
	/// <param name="keyword">For total search (name). For eg: gacold, cantho, ...</param>
	/// <response code="200">
	/// - role: 10 (user), 80 (admin), 100 (root).
	/// - status: 1 (normal), 2 (blocked)
	/// </response>
	[RequireAdminDk]
	[HttpGet, Route(Routes.user_list)]
	public async Task<ActionResult<ApiResponse>> SearchUsers(
		[FromQuery] int page,
		[FromQuery] int item,
		[FromQuery] string? sort_type,
		[FromQuery] string? keyword
	) {
		// Restricts range to avoid spam
		if (page < 1 || item < 1 || item > 100) {
			return new ApiBadRequestResponse("Invalid range");
		}
		return await this.service.SearchUsers(page, item, sort_type, keyword);
	}

	/// <summary>
	/// Update an user.
	/// </summary>
	/// <response code="200"></response>
	[RequireAdminDk]
	[HttpPost, Route(Routes.user_update)]
	public async Task<ActionResult<ApiResponse>> UpdateUser([FromRoute] Guid user_id, [FromBody] UpdateUserRequestBody requestBody) {
		return await this.service.UpdateUser(user_id, requestBody);
	}
}
