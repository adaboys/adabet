namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tool.Compet.Core;

/// Framework will auto convert our result type to client's request Accept-Content type.
/// By default, it implicit convert response R to ActionResult<R>.
/// See https://docs.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-6.0
[ApiController, Route(Routes.api_prefix)]
public class AuthController : BaseController {
	private readonly AuthService service;
	private readonly ILogger<AuthController> logger;

	public AuthController(AuthService service, ILogger<AuthController> logger) {
		this.service = service;
		this.logger = logger;
	}

	/// <summary>
	/// Login an user with email/password.
	/// </summary>
	/// <param name="requestBody">
	/// - client_type: 1 (android), 2 (ios), 3 (web).
	/// </param>
	/// <response code="200"></response>
	[HttpPost, Route(Routes.auth_login)]
	public async Task<ActionResult<ApiResponse>> LogIn([FromBody] LoginRequestBody requestBody) {
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await service.LogIn(requestBody, RequestHelper.GetClientIp(this.Request), RequestHelper.GetUserAgent(this.Request));
	}

	/// <summary>
	/// Login or Signup an user.
	/// </summary>
	/// <param name="requestBody">
	/// - provider: One of: google, facebook.
	/// - id_token: Client get this field after login with the provider (can be null, but must provide access_token)
	/// - access_token: Client get this field after login with the provider (can be null, but must provide id_token)
	/// - client_type: 1 (android), 2 (ios), 3 (web).
	/// </param>
	/// <response code="200"></response>
	[HttpPost, Route(Routes.auth_loginWithProvider)]
	public async Task<ActionResult<ApiResponse>> LogInWithProvider([FromBody] ProviderLoginRequestBody requestBody) {
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await service.LogInWithProvider(requestBody, RequestHelper.GetClientIp(this.Request), RequestHelper.GetUserAgent(this.Request));
	}

	/// <summary>
	/// Request login an user with external wallet.
	/// This is required before call loginWithExternalWallet api.
	/// </summary>
	/// <param name="requestBody">
	/// - wallet_address: Address of external wallet (Yoroi, Nami,...). For eg,.
	/// const wallet_address_in_hex = await this.API.getChangeAddress();
	/// const wallet_address = Address.from_bytes(Buffer.from(wallet_address_in_hex, "hex")).to_bech32()
	/// </param>
	/// <response code="200"></response>
	[HttpPost, Route(Routes.auth_requestLoginWithExternalWallet)]
	public async Task<ActionResult<ApiResponse>> RequestLoginWithExternalWallet([FromBody] RequestLoginWithExternalWalletRequestBody requestBody) {
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await service.RequestLoginWithExternalWallet(requestBody);
	}

	/// <summary>
	/// Login an user with external wallet.
	/// </summary>
	/// <param name="requestBody">
	/// - wallet_address_in_hex: Address-in-hex of external wallet (Yoroi, Nami,...). For eg,.
	/// const wallet_address_in_hex = await this.API.getChangeAddress();
	/// - wallet_address: Address of external wallet (Yoroi, Nami,...). For eg,.
	/// const wallet_address = Address.from_bytes(Buffer.from(wallet_address_in_hex, "hex")).to_bech32()
	/// - signed_signature: Signature obtained from external wallet after user has logged in.
	/// - signed_key: Key obtained from external wallet after user has logged in.
	/// - client_type: 1 (android), 2 (ios), 3 (web).
	/// </param>
	/// <response code="200"></response>
	[HttpPost, Route(Routes.auth_loginWithExternalWallet)]
	public async Task<ActionResult<ApiResponse>> LoginWithExternalWallet([FromBody] LoginWithExternalWalletRequestBody requestBody) {
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await service.LoginWithExternalWallet(requestBody, RequestHelper.GetClientIp(this.Request), RequestHelper.GetUserAgent(this.Request));
	}

	/// <summary>
	/// Silent login an user to get new access_token and refresh_token.
	/// </summary>
	/// <response code="200"></response>
	[HttpPost, Route(Routes.auth_silentLogin)]
	public async Task<ActionResult<ApiResponse>> SilentLogin([FromBody] SilentLoginRequestBody requestBody) {
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await service.SilentLogin(requestBody, RequestHelper.GetClientIp(this.Request), RequestHelper.GetUserAgent(this.Request));
	}

	/// <summary>
	/// Login an user from refresh_token + access_token.
	/// </summary>
	/// <param name="requestBody">
	/// - client_type: 1 (android), 2 (ios), 3 (web).
	/// </param>
	/// <response code="200"></response>
	/// <response code="401"></response>
	[HttpPost, Route(Routes.auth_loginWithToken)]
	public async Task<ActionResult<ApiResponse>> LoginWithToken([FromBody] LoginWithTokenRequestBody requestBody) {
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await service.LoginWithToken(requestBody, RequestHelper.GetClientIp(this.Request), RequestHelper.GetUserAgent(this.Request));
	}

	/// <summary>
	/// Logout an user from current place or everywhere.
	/// </summary>
	/// <param name="requestBody">
	/// - logout_everywhere: Default is false. If provide true, this will logout the user from all places.
	/// </param>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.auth_logout)]
	public async Task<ActionResult<ApiResponse>> LogOut([FromBody] LogoutRequestBody requestBody) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await service.LogOut(userId.Value, requestBody, RequestHelper.GetClientIp(this.Request), RequestHelper.GetUserAgent(this.Request));
	}

	/// <summary>
	/// Request password-reset for given email.
	/// </summary>
	/// <param name="requestBody"></param>
	/// <response code="200"></response>
	[HttpPost, Route(Routes.auth_password_reset_request)]
	public async Task<ActionResult<ApiResponse>> RequestResetPassword([FromBody] RequestResetPasswordRequestBody requestBody) {
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await service.RequestResetPassword(requestBody.email);
	}

	/// <summary>
	/// Reset password.
	/// </summary>
	/// <param name="requestBody"></param>
	/// <response code="200"></response>
	[HttpPost, Route(Routes.auth_password_reset_confirm)]
	public async Task<ActionResult<ApiResponse>> ConfirmResetPassword([FromBody] ConfirmResetPasswordRequestBody requestBody) {
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await service.ConfirmResetPassword(requestBody, RequestHelper.GetClientIp(this.Request), RequestHelper.GetUserAgent(this.Request));
	}

	[HttpGet, Route(ServiceRoutes.user_auth_info)]
	public async Task<ActionResult<ApiResponse>> GetUserAuthInfo([FromQuery] string api_key, [FromQuery] string refresh_token) {
		return await service.GetUserAuthInfo(api_key, refresh_token);
	}
}
