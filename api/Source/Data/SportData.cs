namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Sport_ListResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "sports")]
		public Sport[] sports { get; set; }
	}

	public class Sport {
		[JsonPropertyName(name: "id")]
		public int id { get; set; }

		[JsonPropertyName(name: "name")]
		public string name { get; set; }

		[JsonPropertyName(name: "country_count")]
		public int country_count { get; set; }
	}
}


public class Sport_MatchesResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "matches")]
		public Match[] matches { get; set; }
	}

	public class Match {
		[JsonPropertyName(name: "id")]
		public long id { get; set; }

		[JsonPropertyName(name: "score1")]
		public int score1 { get; set; }

		[JsonPropertyName(name: "score2")]
		public int score2 { get; set; }

		[JsonPropertyName(name: "league")]
		public string league { get; set; }

		[JsonPropertyName(name: "team1")]
		public string team1 { get; set; }

		[JsonPropertyName(name: "team2")]
		public string team2 { get; set; }

		[JsonPropertyName(name: "start_at")]
		public DateTime start_at { get; set; }

		[JsonPropertyName(name: "cur_time")]
		public int cur_time { get; set; }

		[JsonPropertyName(name: "markets")]
		public List<Market> markets { get; set; }
	}
}

public class Sport_UpcomingMatchesResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "page_pos")]
		public int page_pos { get; set; }

		[JsonPropertyName(name: "page_count")]
		public int page_count { get; set; }

		[JsonPropertyName(name: "total_item_count")]
		public int total_item_count { get; set; }

		[JsonPropertyName(name: "matches")]
		public Match[] matches { get; set; }
	}

	public class Match {
		[JsonPropertyName(name: "id")]
		public long id { get; set; }

		[JsonPropertyName(name: "score1")]
		public int score1 { get; set; }

		[JsonPropertyName(name: "score2")]
		public int score2 { get; set; }

		[JsonPropertyName(name: "league")]
		public string league { get; set; }

		[JsonPropertyName(name: "team1")]
		public string team1 { get; set; }

		[JsonPropertyName(name: "team2")]
		public string team2 { get; set; }

		[JsonPropertyName(name: "start_at")]
		public DateTime start_at { get; set; }

		[JsonPropertyName(name: "cur_time")]
		public int cur_time { get; set; }

		[JsonPropertyName(name: "markets")]
		public List<Market> markets { get; set; }
	}
}

public class Sport_PlaceBetRequestBody {
	[Required]
	[MinLength(1)]
	[JsonPropertyName(name: "bets")]
	public Bet[] bets { get; set; }

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
		[Range(2, 1000)]
		[JsonPropertyName(name: "ada_amount")]
		public decimal ada_amount { get; set; }
	}
}

public class Sport_GetQuickLinksResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "links")]
		public Link[] links { get; set; }
	}

	public class Link {
		[JsonPropertyName(name: "name")]
		public string name { get; set; }
	}
}

public class Sport_GetHighlightMatchesResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "matches")]
		public Match[] matches { get; set; }
	}

	public class Match {
		internal List<Market> tmp_markets;

		[JsonPropertyName(name: "id")]
		public long id { get; set; }

		[JsonPropertyName(name: "score1")]
		public int score1 { get; set; }

		[JsonPropertyName(name: "score2")]
		public int score2 { get; set; }

		[JsonPropertyName(name: "league")]
		public string league { get; set; }

		[JsonPropertyName(name: "team1")]
		public string team1 { get; set; }

		[JsonPropertyName(name: "team2")]
		public string team2 { get; set; }

		[JsonPropertyName(name: "start_at")]
		public DateTime start_at { get; set; }

		[JsonPropertyName(name: "cur_time")]
		public int cur_time { get; set; }

		[JsonPropertyName(name: "market")]
		public Market? market { get; set; }
	}
}

public class Sport_GetTopMatchesResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "matches")]
		public Match[] matches { get; set; }
	}

	public class Match {
		[JsonPropertyName(name: "id")]
		public long id { get; set; }

		[JsonPropertyName(name: "score1")]
		public int score1 { get; set; }

		[JsonPropertyName(name: "score2")]
		public int score2 { get; set; }

		[JsonPropertyName(name: "league")]
		public string league { get; set; }

		[JsonPropertyName(name: "team1")]
		public string team1 { get; set; }

		[JsonPropertyName(name: "team2")]
		public string team2 { get; set; }

		[JsonPropertyName(name: "start_at")]
		public DateTime start_at { get; set; }

		[JsonPropertyName(name: "cur_time")]
		public int cur_time { get; set; }

		[JsonPropertyName(name: "market")]
		public Market? market { get; set; }

		[JsonIgnore]
		public List<Market> tmp_markets { get; set; }
	}
}
