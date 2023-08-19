namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Sport_BasePlaceBetPayload {
	[Required]
	[MinLength(1)]
	[JsonPropertyName(name: "bets")]
	public _UserSportDataBet[] bets { get; set; }

	[Required]
	[JsonPropertyName(name: "bet_currency")]
	public int bet_currency { get; set; }
}

public class Sport_PlaceBetPayload : Sport_BasePlaceBetPayload {
	[JsonPropertyName(name: "reward_address")]
	public string? reward_address { get; set; } = null;
}

public class RequestBetViaExtWalletPayload : Sport_BasePlaceBetPayload {
}

public class RequestBetViaExtWalletResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "order_code")]
		public string order_code { get; set; }

		[JsonPropertyName(name: "timeout")]
		public int timeout { get; set; }

		[JsonPropertyName(name: "receiver_address")]
		public string receiver_address { get; set; }

		[JsonPropertyName(name: "total_bet_amount")]
		public decimal total_bet_amount { get; set; }
	}
}

public class ReportBetViaExtWalletPayload {
	[Required]
	[JsonPropertyName(name: "order_code")]
	public string order_code { get; set; }

	[Required]
	[JsonPropertyName(name: "sender_address")]
	public string sender_address { get; set; }

	[Required]
	[JsonPropertyName(name: "payment_tx_hash")]
	public string payment_tx_hash { get; set; }
}

public class _UserSportDataBet {
	[Required]
	[JsonPropertyName(name: "match_id")]
	public long match_id { get; set; }

	[Required]
	[MinLength(1)]
	[JsonPropertyName(name: "markets")]
	public _Market[] markets { get; set; }

	public class _Market {
		/// For eg,. result, handicap, home_total, away_total,...
		[Required]
		[JsonPropertyName(name: "name")]
		public string name { get; set; }

		[Required]
		[MinLength(1)]
		[JsonPropertyName(name: "odds")]
		public _Odd[] odds { get; set; }
	}

	public class _Odd {
		/// For eg,. home_win, over, yes,...
		[Required]
		[JsonPropertyName(name: "name")]
		public string name { get; set; }

		/// For eg,. 1.32, 2.01, 4.19,...
		[Required]
		[JsonPropertyName(name: "value")]
		public decimal value { get; set; }

		[Required]
		[Range(2, 1000)] //todo need dynamic value from setting or db
		[JsonPropertyName(name: "bet_amount")]
		public decimal bet_amount { get; set; }
	}
}

public class Sport_GetBetHistoriesResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "page_pos")]
		public int page_pos { get; set; }

		[JsonPropertyName(name: "page_count")]
		public int page_count { get; set; }

		[JsonPropertyName(name: "total_item_count")]
		public int total_item_count { get; set; }

		[JsonPropertyName(name: "bets")]
		public Bet[] bets { get; set; }
	}

	public class Bet {
		[JsonPropertyName(name: "sport")]
		public int sport_id { get; set; }

		[JsonPropertyName(name: "country")]
		public string country { get; set; }

		[JsonPropertyName(name: "league")]
		public string league { get; set; }

		[JsonPropertyName(name: "team1")]
		public string team1 { get; set; }

		[JsonPropertyName(name: "team2")]
		public string team2 { get; set; }

		[JsonPropertyName(name: "start_at")]
		public DateTime start_at { get; set; }

		[JsonPropertyName(name: "score1")]
		public short score1 { get; set; }

		[JsonPropertyName(name: "score2")]
		public short score2 { get; set; }

		[JsonPropertyName(name: "status")]
		public int status { get; set; }

		[JsonPropertyName(name: "timer")]
		public short timer { get; set; }

		[JsonPropertyName(name: "ticket")]
		public long ticket { get; set; }

		[JsonPropertyName(name: "market")]
		public string bet_market_name { get; set; }

		[JsonPropertyName(name: "odd")]
		public decimal bet_odd_value { get; set; }

		[JsonPropertyName(name: "bet_tx_id")]
		public string? bet_tx_id { get; set; }

		[JsonPropertyName(name: "bet_result")]
		public int bet_result { get; set; }

		[JsonPropertyName(name: "coin")]
		public string coin_name { get; set; }

		[JsonPropertyName(name: "staked")]
		public decimal staked { get; set; }

		[JsonPropertyName(name: "potential_reward")]
		public decimal potential_reward { get; set; }

		[JsonPropertyName(name: "reward_status")]
		public int reward_status { get; set; }

		[JsonPropertyName(name: "reward_tx_id")]
		public string? reward_tx_id { get; set; }
	}
}

public class GetBadgesResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "fav_cnt")]
		public int favorite_count { get; set; }

		[JsonPropertyName(name: "bet_cnt")]
		public int bet_count { get; set; }
	}
}
