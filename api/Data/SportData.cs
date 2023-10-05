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
		internal int _tmpOrder;

		[JsonPropertyName(name: "id")]
		public int id { get; set; }

		[JsonPropertyName(name: "name")]
		public string name { get; set; }

		[JsonPropertyName(name: "leagues")]
		public List<League> leagues { get; set; } = new();
	}

	public class League {
		internal int _tmpSportId;

		[JsonPropertyName(name: "id")]
		public long id { get; set; }

		[JsonPropertyName(name: "name")]
		public string name { get; set; }
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

		[JsonPropertyName(name: "sport")]
		public int sport_id { get; set; }

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

		[JsonPropertyName(name: "s")]
		public int status { get; set; }

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

		[JsonPropertyName(name: "sport")]
		public int sport_id { get; set; }

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

		[JsonPropertyName(name: "s")]
		public int status { get; set; }

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

		[JsonPropertyName(name: "sport")]
		public int sport_id { get; set; }

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

		[JsonPropertyName(name: "s")]
		public int status { get; set; }

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

		[JsonPropertyName(name: "sport")]
		public int sport_id { get; set; }

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

		[JsonPropertyName(name: "s")]
		public int status { get; set; }

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

public class Sport_GetMatchHistoryResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {

		[JsonPropertyName(name: "summary")]
		public Summary summary { get; set; }

		[JsonPropertyName(name: "next_meetings")]
		public NextMeeting next_meetings { get; set; }

		[JsonPropertyName(name: "last_meetings")]
		public LastMeeting last_meetings { get; set; }

		[JsonPropertyName(name: "last_matches")]
		public Matches last_matches { get; set; }

		[JsonPropertyName(name: "next_matches")]
		public Matches next_matches { get; set; }
	}

	public class Summary {
		[JsonPropertyName(name: "draw")]
		public int draw { get; set; } = new();

		[JsonPropertyName(name: "home")]
		public SummaryDetail home { get; set; } = new();

		[JsonPropertyName(name: "away")]
		public SummaryDetail away { get; set; } = new();

		public class SummaryDetail {
			[JsonPropertyName(name: "victories")]
			public int victories { get; set; }

			[JsonPropertyName(name: "tt_goals")]
			public int tt_goals { get; set; }

			[JsonPropertyName(name: "avg_goal_match")]
			public float avg_goal_match { get; set; }

			[JsonPropertyName(name: "highest_win")]
			public string highest_win { get; set; }

			[JsonPropertyName(name: "performance")]
			public List<Performance> performance { get; set; } = new();
		}

		public class Performance {
			[JsonPropertyName(name: "s1")]
			public int s1 { get; set; }

			[JsonPropertyName(name: "s2")]
			public int s2 { get; set; }

			[JsonPropertyName(name: "time")]
			public string time { get; set; }
		}
	}

	public class LastMeeting {
		[JsonPropertyName(name: "list")]
		public List<LastMeetingDetail> list { get; set; } = new();
	}

	public class LastMeetingDetail {
		[JsonPropertyName(name: "ss")]
		public string scores { get; set; }

		[JsonPropertyName(name: "time")]
		public string time { get; set; }

		[JsonPropertyName(name: "league")]
		public string league { get; set; }

		[JsonPropertyName(name: "home")]
		public Team home { get; set; }

		[JsonPropertyName(name: "away")]
		public Team away { get; set; }
	}

	public class NextMeeting {
		[JsonPropertyName(name: "list")]
		public List<NextMeetingDetail> list { get; set; } = new();
	}

	public class NextMeetingDetail {
		[JsonPropertyName(name: "ss")]
		public string scores { get; set; }

		[JsonPropertyName(name: "time")]
		public string time { get; set; }

		[JsonPropertyName(name: "league")]
		public string league { get; set; }

		[JsonPropertyName(name: "home")]
		public Team home { get; set; }

		[JsonPropertyName(name: "away")]
		public Team away { get; set; }
	}

	public class Matches {
		[JsonPropertyName(name: "homes")]
		public List<Home> homes { get; set; } = new();

		[JsonPropertyName(name: "aways")]
		public List<Away> aways { get; set; } = new();
	}

	public class Home {
		[JsonPropertyName(name: "league")]
		public string league { get; set; }

		[JsonPropertyName(name: "time")]
		public string time { get; set; }

		[JsonPropertyName(name: "status")]
		public int status { get; set; }

		[JsonPropertyName(name: "ss")]
		public string scores { get; set; }

		[JsonPropertyName(name: "home")]
		public Team home { get; set; }

		[JsonPropertyName(name: "away")]
		public Team away { get; set; }
	}

	public class Away {
		[JsonPropertyName(name: "league")]
		public string league { get; set; }

		[JsonPropertyName(name: "time")]
		public string time { get; set; }

		[JsonPropertyName(name: "status")]
		public int status { get; set; }

		[JsonPropertyName(name: "ss")]
		public string scores { get; set; }

		[JsonPropertyName(name: "home")]
		public Team home { get; set; }

		[JsonPropertyName(name: "away")]
		public Team away { get; set; }
	}

	public class Team {
		[JsonPropertyName(name: "name")]
		public string name { get; set; }

		[JsonPropertyName(name: "img")]
		public string img { get; set; }
	}
}
