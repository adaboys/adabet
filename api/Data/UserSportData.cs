namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Sport_PlaceBetRequestBody {
	[Required]
	[MinLength(1)]
	[JsonPropertyName(name: "bets")]
	public Bet[] bets { get; set; }

	[Required]
	[JsonPropertyName(name: "bet_currency")]
	public int bet_currency { get; set; }

	[JsonPropertyName(name: "reward_address")]
	public string? reward_address { get; set; } = null;

	public class Bet {
		[Required]
		[JsonPropertyName(name: "match_id")]
		public long match_id { get; set; }

		[Required]
		[MinLength(1)]
		[JsonPropertyName(name: "markets")]
		public Market[] markets { get; set; }
	}

	public class Market {
		/// For eg,. result, handicap, home_total, away_total,...
		[Required]
		[JsonPropertyName(name: "name")]
		public string name { get; set; }

		[Required]
		[MinLength(1)]
		[JsonPropertyName(name: "odds")]
		public Odd[] odds { get; set; }
	}

	public class Odd {
		/// For eg,. home_win, over, yes,...
		[Required]
		[JsonPropertyName(name: "name")]
		public string name { get; set; }

		/// For eg,. 1.32, 2.01, 4.19,...
		[Required]
		[JsonPropertyName(name: "value")]
		public decimal value { get; set; }

		[Required]
		[Range(2, 500)]
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
		public SportMatchModelConst.TimeStatus status { get; set; }

		[JsonPropertyName(name: "timer")]
		public short timer { get; set; }

		[JsonPropertyName(name: "ticket")]
		public long ticket { get; set; }

		[JsonPropertyName(name: "market")]
		public string bet_market_name { get; set; }

		[JsonPropertyName(name: "odd")]
		public decimal bet_odd_value { get; set; }

		[JsonPropertyName(name: "staked")]
		public decimal staked { get; set; }

		[JsonPropertyName(name: "coin")]
		public string coin { get; set; }
	}
}
