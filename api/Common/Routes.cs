namespace App;

/// Api routes for web.
public partial class Routes {
	public const string api_prefix = "api";

	/// [Master]


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
	public const string user_avatar_update = "user/avatar/update"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string user_external_wallet_requestLink = "user/external_wallet/{wallet_address}/requestLink"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string user_external_wallet_link = "user/external_wallet/{wallet_address}/link"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string user_external_wallet_linked_wallets = "user/external_wallet/linked_wallets"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string user_external_wallet_unlink = "user/external_wallet/{wallet_address}/unlink"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}

	public const string user_currency_list = "user/currency/list"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 30}}

	/// [User] -> [Favorite]
	public const string user_match_favorite_toggle = "user/match/{match_id}/favorite/toggle"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string user_sport_favorite_list = "user/sport/{sport_id}/favorite/list"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}

	/// [Sport]
	public const string sports = "sports"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_league_quick_links = "sport/{sport_id}/league/quick_links"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_matches_live = "sport/{sport_id}/matches/live"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_matches_upcoming = "sport/{sport_id}/matches/upcoming"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_matches_highlight = "sport/{sport_id}/matches/highlight"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_matches_top = "sport/{sport_id}/matches/top"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_bet_place = "sport/{sport_id}/bet/place"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string sport_user_bet_histories = "sport/{sport_id}/user/bet/histories"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}

	public const string user_sport_badges = "user/sport/{sport_id}/badges"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}

	public const string sport_prediction_match_list = "sport/{sport_id}/prediction/match/list"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_prediction_match_detail = "sport/prediction/match/{match_id}/detail"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_prediction_match_predict = "sport/prediction/match/{match_id}/predict"; // {"methods":["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string sport_prediction_match_predicted_users = "sport/prediction/match/{match_id}/predicted_users"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 5}}
	public const string sport_prediction_leaderboard = "sport/prediction/leaderboard"; // {"methods":["GET"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}, "cache": {"timeout": 10}}

	public const string coin_withdraw_prepare = "coin/withdraw/prepare"; // {"methods": ["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
	public const string coin_withdraw_actual = "coin/withdraw/actual"; // {"methods": ["POST"], "rate_limit": {"period": 1.0, "period_timespan": 2.0, "limit": 5}}
}
