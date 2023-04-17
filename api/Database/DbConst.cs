namespace App;

public class DbConst {
	/// [System]
	public const string table_sys_wallet = "sys_wallet";

	/// [Master]
	public const string table_mst_app = "mst_app";
	public const string table_mst_country = "mst_country";
	public const string table_mst_currency = "mst_currency";
	public const string table_mst_exchange_rate = "mst_exchange_rate";

	/// [User]
	public const string table_user = "user";
	public const string table_user_wallet = "user_wallet";
	public const string table_user_auth = "user_auth";

	/// [Sport]
	public const string table_mst_sport = "mst_sport";
	public const string table_sport_team = "sport_team";
	public const string table_sport_league = "sport_league";
	public const string table_sport_match = "sport_match";
	public const string table_sport_user_bet = "sport_user_bet";
	public const string table_sport_winner_bet_reward_tx = "sport_winner_bet_reward_tx";

	public const string table_sport_predict_user = "sport_predict_user";
}
