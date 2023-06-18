namespace App;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class UpdateMatchRequestBody {
	[JsonPropertyName(name: "predictable")]
	public bool? predictable { get; set; }
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

		[JsonPropertyName(name: "created_at")]
		public DateTime created_at { get; set; }

		[JsonPropertyName(name: "timer")]
		public int timer { get; set; }

		[JsonPropertyName(name: "predictable")]
		public bool predictable { get; set; }

		// [JsonPropertyName(name: "markets")]
		// public List<Market>? markets { get; set; }
	}
}
