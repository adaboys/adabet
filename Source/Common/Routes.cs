namespace App;

/// Api routes for web.
public partial class Routes {
	public const string api_prefix = "api";

	/// [App]
	public const string app_info = "app/{os_type}/info"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 10}}

	/// [Auth]
	public const string auth_login = "auth/login"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string auth_silentLogin = "auth/silentLogin"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string auth_loginWithProvider = "auth/loginWithProvider"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string auth_loginWithToken = "auth/loginWithToken"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string auth_requestLoginWithExternalWallet = "auth/requestLoginWithExternalWallet"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string auth_loginWithExternalWallet = "auth/loginWithExternalWallet"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string auth_logout = "auth/logout"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string auth_token_validate = "auth/token/validate"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string auth_password_reset_request = "auth/password_reset/request"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string auth_password_reset_confirm = "auth/password_reset/confirm"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}

	/// [User]
	public const string user_register_attempt = "user/register/attempt"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string user_register_confirm = "user/register/confirm"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string user_password_change = "user/password/change"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string user_profile = "user/profile"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string user_balance = "user/balance"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string user_identity_update = "user/identity/update"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string user_external_wallet_requestLink = "user/external_wallet/{wallet_address}/requestLink"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string user_external_wallet_link = "user/external_wallet/{wallet_address}/link"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string user_external_wallet_linked_wallets = "user/external_wallet/linked_wallets"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string user_external_wallet_unlink = "user/external_wallet/{wallet_address}/unlink"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}

	/// [Sport]
	public const string sports = "sports"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_league_quick_links = "sport/{sport_id}/league/quick_links"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_matches_live = "sport/{sport_id}/matches/live"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_matches_upcoming = "sport/{sport_id}/matches/upcoming"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_matches_highlight = "sport/{sport_id}/matches/highlight"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_matches_top = "sport/{sport_id}/matches/top"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_bet_place = "sport/{sport_id}/bet/place"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
}
