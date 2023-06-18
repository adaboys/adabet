namespace App;

/// Api routes for admin.
public partial class Routes {
	public const string api_prefix = "api";

	/// [Auth]
	public const string auth_login = "auth/login";
	public const string auth_silentLogin = "auth/silentLogin";
	// public const string auth_loginWithProvider = "auth/loginWithProvider";
	// public const string auth_loginWithToken = "auth/loginWithToken";
	// public const string auth_requestLoginWithExternalWallet = "auth/requestLoginWithExternalWallet";
	// public const string auth_loginWithExternalWallet = "auth/loginWithExternalWallet";
	public const string auth_logout = "auth/logout";
	// public const string auth_token_validate = "auth/token/validate";
	public const string auth_password_reset_request = "auth/password_reset/request";
	public const string auth_password_reset_confirm = "auth/password_reset/confirm";

	/// [Admin]
	public const string admin_profile = "admin/profile";

	/// [User]
	// public const string user_register_attempt = "user/register/attempt";
	// public const string user_register_confirm = "user/register/confirm";
	// public const string user_password_change = "user/password/change";
	// public const string user_profile = "user/profile";
	// public const string user_balance = "user/balance";
	// public const string user_identity_update = "user/identity/update";
	// public const string user_avatar_update = "user/avatar/update";

	public const string user_list = "user/list";
	public const string user_update = "user/{user_id}/update";

	/// [Sport]
	public const string sport_match_list = "sport/{sport_id}/match/list";
	public const string sport_match_update = "sport/match/{match_id}/update";
}
