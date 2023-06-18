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

		[JsonPropertyName(name: "s1")]
		public int score1 { get; set; }

		[JsonPropertyName(name: "s2")]
		public int score2 { get; set; }

		[JsonPropertyName(name: "t1")]
		public string team1 { get; set; }

		[JsonPropertyName(name: "t2")]
		public string team2 { get; set; }

		[JsonPropertyName(name: "img1")]
		public string? image1 { get; set; }

		[JsonPropertyName(name: "img2")]
		public string? image2 { get; set; }

		[JsonPropertyName(name: "country")]
		public string? country { get; set; }

		[JsonPropertyName(name: "league")]
		public string league { get; set; }

		[JsonPropertyName(name: "esport")]
		public bool is_esport { get; set; }

		[JsonPropertyName(name: "start_at")]
		public DateTime start_at { get; set; }

		[JsonPropertyName(name: "timer")]
		public Timer timer { get; set; }

		[JsonPropertyName(name: "markets")]
		public List<Market>? markets { get; set; }

		[JsonPropertyName(name: "favorited")]
		public bool favorited { get; set; }
	}

	public class Timer {
		[JsonPropertyName(name: "t")]
		public short time { get; set; }

		[JsonPropertyName(name: "tt")]
		public short total_timer { get; set; }

		[JsonPropertyName(name: "b")]
		public bool is_break { get; set; }

		[JsonPropertyName(name: "i")]
		public short injury_time { get; set; }
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

		[JsonPropertyName(name: "s1")]
		public int score1 { get; set; }

		[JsonPropertyName(name: "s2")]
		public int score2 { get; set; }

		[JsonPropertyName(name: "country")]
		public string? country { get; set; }

		[JsonPropertyName(name: "league")]
		public string league { get; set; }

		[JsonPropertyName(name: "esport")]
		public bool is_esport { get; set; }

		[JsonPropertyName(name: "t1")]
		public string team1 { get; set; }

		[JsonPropertyName(name: "t2")]
		public string team2 { get; set; }

		[JsonPropertyName(name: "img1")]
		public string? image1 { get; set; }

		[JsonPropertyName(name: "img2")]
		public string? image2 { get; set; }

		[JsonPropertyName(name: "start_at")]
		public DateTime start_at { get; set; }

		[JsonPropertyName(name: "timer")]
		public Timer timer { get; set; }

		[JsonPropertyName(name: "markets")]
		public List<Market>? markets { get; set; }

		[JsonPropertyName(name: "favorited")]
		public bool favorited { get; set; }
	}

	public class Timer {
		[JsonPropertyName(name: "t")]
		public short time { get; set; }

		[JsonPropertyName(name: "tt")]
		public short total_timer { get; set; }

		[JsonPropertyName(name: "b")]
		public bool is_break { get; set; }

		[JsonPropertyName(name: "i")]
		public short injury_time { get; set; }
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
		internal List<Market>? tmp_markets;

		[JsonPropertyName(name: "id")]
		public long id { get; set; }

		[JsonPropertyName(name: "country")]
		public string? country { get; set; }

		[JsonPropertyName(name: "league")]
		public string league { get; set; }

		[JsonPropertyName(name: "esport")]
		public bool is_esport { get; set; }

		[JsonPropertyName(name: "s1")]
		public int score1 { get; set; }

		[JsonPropertyName(name: "s2")]
		public int score2 { get; set; }

		[JsonPropertyName(name: "t1")]
		public string team1 { get; set; }

		[JsonPropertyName(name: "t2")]
		public string team2 { get; set; }

		[JsonPropertyName(name: "img1")]
		public string? image1 { get; set; }

		[JsonPropertyName(name: "img2")]
		public string? image2 { get; set; }

		[JsonPropertyName(name: "start_at")]
		public DateTime start_at { get; set; }

		[JsonPropertyName(name: "timer")]
		public Timer timer { get; set; }

		[JsonPropertyName(name: "market")]
		public Market? market { get; set; }
	}

	public class Timer {
		[JsonPropertyName(name: "t")]
		public short time { get; set; }

		[JsonPropertyName(name: "tt")]
		public short total_timer { get; set; }

		[JsonPropertyName(name: "b")]
		public bool is_break { get; set; }

		[JsonPropertyName(name: "i")]
		public short injury_time { get; set; }
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

		[JsonPropertyName(name: "country")]
		public string? country { get; set; }

		[JsonPropertyName(name: "league")]
		public string league { get; set; }

		[JsonPropertyName(name: "esport")]
		public bool is_esport { get; set; }

		[JsonPropertyName(name: "s1")]
		public int score1 { get; set; }

		[JsonPropertyName(name: "s2")]
		public int score2 { get; set; }

		[JsonPropertyName(name: "t1")]
		public string team1 { get; set; }

		[JsonPropertyName(name: "t2")]
		public string team2 { get; set; }

		[JsonPropertyName(name: "img1")]
		public string? image1 { get; set; }

		[JsonPropertyName(name: "img2")]
		public string? image2 { get; set; }

		[JsonPropertyName(name: "start_at")]
		public DateTime start_at { get; set; }

		[JsonPropertyName(name: "timer")]
		public Timer timer { get; set; }

		[JsonPropertyName(name: "markets")]
		public List<Market>? markets { get; set; }

		[JsonPropertyName(name: "favorited")]
		public bool favorited { get; set; }
	}

	public class Timer {
		[JsonPropertyName(name: "t")]
		public short time { get; set; }

		[JsonPropertyName(name: "tt")]
		public short total_timer { get; set; }

		[JsonPropertyName(name: "b")]
		public bool is_break { get; set; }

		[JsonPropertyName(name: "i")]
		public short injury_time { get; set; }
	}
}
