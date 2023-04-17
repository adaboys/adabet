namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class GetSportPredictionMatchesResponse : ApiSuccessResponse {
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

		[JsonPropertyName(name: "start_at")]
		public DateTime start_at { get; set; }

		[JsonPropertyName(name: "timer")]
		public int timer { get; set; }

		[JsonPropertyName(name: "predicted_at")]
		public DateTime? predicted_at { get; set; }
	}
}


public class Sport_PredictMatchRequestBody {
	[Required]
	[JsonPropertyName(name: "score1")]
	public int score1 { get; set; }

	[Required]
	[JsonPropertyName(name: "score2")]
	public int score2 { get; set; }

	[JsonPropertyName(name: "reward_address")]
	public string? reward_address { get; set; }
}


public class GetSportPredictionUserListOnMatchResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "page_pos")]
		public int page_pos { get; set; }

		[JsonPropertyName(name: "page_count")]
		public int page_count { get; set; }

		[JsonPropertyName(name: "total_item_count")]
		public int total_item_count { get; set; }

		[JsonPropertyName(name: "predictions")]
		public Prediction[] predictions { get; set; }
	}

	public class Prediction {
		[JsonPropertyName(name: "player_name")]
		public string player_name { get; set; }

		[JsonPropertyName(name: "predict_score1")]
		public int predict_score1 { get; set; }

		[JsonPropertyName(name: "predict_score2")]
		public int predict_score2 { get; set; }

		[JsonPropertyName(name: "predicted_at")]
		public DateTime? predicted_at { get; set; }

		[JsonPropertyName(name: "prediction_rank")]
		public int prediction_rank { get; set; }

		[JsonPropertyName(name: "reward_coin_name")]
		public string reward_coin_name { get; set; }

		[JsonPropertyName(name: "reward_coin_amount")]
		public decimal reward_coin_amount { get; set; }

		[JsonPropertyName(name: "reward_delivery_status")]
		public int reward_delivery_status { get; set; }
	}
}

public class GetLeaderboardResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "leaderboard")]
		public LeaderboardItem[] leaderboard { get; set; }
	}

	public class LeaderboardItem {
		internal Guid tmp_userId;

		[JsonPropertyName(name: "player")]
		public string? player { get; set; }

		[JsonPropertyName(name: "reward_sum")]
		public decimal reward_sum { get; set; }
	}
}
