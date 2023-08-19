namespace App;

using System.Text.Json.Serialization;

public class BetsapiData_SoccerMatchDetail {
	public int success { get; set; }
	public List<Result> results { get; set; }

	[JsonIgnore]
	public bool succeed => this.success == 1;
	[JsonIgnore]
	public bool failed => this.success != 1;

	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class _2 {
		public string home { get; set; }
		public string away { get; set; }
	}

	public class Away {
		public long id { get; set; }
		public string name { get; set; }
		public string image_id { get; set; }
		public string cc { get; set; }
	}

	public class Event {
		public string id { get; set; }
		public string text { get; set; }
	}

	public class Extra {
		public int length { get; set; }
		public string home_pos { get; set; }
		public string away_pos { get; set; }
		public string numberofperiods { get; set; }
		public string periodlength { get; set; }
		public StadiumData stadium_data { get; set; }
		public string round { get; set; }
	}

	public class Home {
		public long id { get; set; }
		public string name { get; set; }
		public string image_id { get; set; }
		public string cc { get; set; }
	}

	public class League {
		public string id { get; set; }
		public string name { get; set; }
		public string cc { get; set; }
	}

	public class Result {
		public long id { get; set; }
		public long sport_id { get; set; }
		public string time { get; set; }
		public int time_status { get; set; }
		public League league { get; set; }
		public Home home { get; set; }
		public Away away { get; set; }
		public string ss { get; set; }
		public Timer timer { get; set; }
		public Scores scores { get; set; }
		public Stats stats { get; set; }
		public Extra extra { get; set; }
		public List<Event> events { get; set; }
		public int has_lineup { get; set; }
		public string inplay_created_at { get; set; }
		public string inplay_updated_at { get; set; }
		public string confirmed_at { get; set; }
		public string bet365_id { get; set; }
	}

	public class Scores {
		[JsonPropertyName("2")]
		public _2 _2 { get; set; }
	}

	public class StadiumData {
		public string id { get; set; }
		public string name { get; set; }
		public string city { get; set; }
		public string country { get; set; }
		public string capacity { get; set; }
		public string googlecoords { get; set; }
	}

	public class Stats {
		public List<string> attacks { get; set; }
		public List<string> ball_safe { get; set; }
		public List<string> corners { get; set; }
		public List<string> dangerous_attacks { get; set; }
		public List<string> goals { get; set; }
		public List<string> off_target { get; set; }
		public List<string> on_target { get; set; }
		public List<string> penalties { get; set; }
		public List<string> possession_rt { get; set; }
		public List<string> redcards { get; set; }
		public List<string> substitutions { get; set; }
		public List<string> yellowcards { get; set; }
		public List<string> yellowred_cards { get; set; }
	}

	public class Timer {
		public int tm { get; set; }
		public int ts { get; set; }
		public string tt { get; set; }
		public short ta { get; set; }
		public int md { get; set; }
	}
}

public class BetsapiData_SoccerMatchDetail_Upcoming {
	public int success { get; set; }
	public List<Result> results { get; set; }

	[JsonIgnore]
	public bool succeed => this.success == 1;
	[JsonIgnore]
	public bool failed => this.success != 1;

	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class Away {
		public string id { get; set; }
		public string name { get; set; }
		public string image_id { get; set; }
		public string cc { get; set; }
	}

	public class Extra {
		public string numberofperiods { get; set; }
		public string periodlength { get; set; }
		public StadiumData stadium_data { get; set; }
		public int length { get; set; }
		public string round { get; set; }
	}

	public class Home {
		public string id { get; set; }
		public string name { get; set; }
		public string image_id { get; set; }
		public string cc { get; set; }
	}

	public class League {
		public string id { get; set; }
		public string name { get; set; }
		public string cc { get; set; }
	}

	public class Result {
		public long id { get; set; }
		public long sport_id { get; set; }
		public long time { get; set; }
		public int time_status { get; set; }
		public League league { get; set; }
		public Home home { get; set; }
		public Away away { get; set; }
		public string? ss { get; set; }
		public Extra extra { get; set; }
		public int has_lineup { get; set; }
		public string confirmed_at { get; set; }
	}

	public class StadiumData {
		public string id { get; set; }
		public string name { get; set; }
		public string city { get; set; }
		public string country { get; set; }
		public string capacity { get; set; }
		public string googlecoords { get; set; }
	}
}

public class BetsapiData_TennisMatchDetail {
	public int success { get; set; }
	public List<Result> results { get; set; }

	[JsonIgnore]
	public bool succeed => this.success == 1;
	[JsonIgnore]
	public bool failed => this.success != 1;

	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class _1 {
		public string home { get; set; }
		public string away { get; set; }
	}

	public class _2 {
		public string home { get; set; }
		public string away { get; set; }
	}

	public class _3 {
		public string home { get; set; }
		public string away { get; set; }
	}

	public class Away {
		public string id { get; set; }
		public string name { get; set; }
		public int image_id { get; set; }
		public string? cc { get; set; }
	}

	public class Event {
		public string id { get; set; }
		public string text { get; set; }
	}

	public class Extra {
		public string bestofsets { get; set; }
		public StadiumData stadium_data { get; set; }
		public string round { get; set; }
		public string ground { get; set; }
	}

	public class Home {
		public string id { get; set; }
		public string name { get; set; }
		public int image_id { get; set; }
		public string? cc { get; set; }
	}

	public class League {
		public string id { get; set; }
		public string name { get; set; }
		public object cc { get; set; }
	}

	public class Result {
		public long id { get; set; }
		public string sport_id { get; set; }
		public string time { get; set; }
		public int time_status { get; set; }
		public League league { get; set; }
		public Home home { get; set; }
		public Away away { get; set; }
		public string ss { get; set; }
		public string points { get; set; }
		public string playing_indicator { get; set; }
		public Stats stats { get; set; }
		public List<Event> events { get; set; }
		public Scores scores { get; set; }
		public Extra extra { get; set; }
		public string inplay_created_at { get; set; }
		public string inplay_updated_at { get; set; }
		public string confirmed_at { get; set; }
		public string bet365_id { get; set; }
	}

	public class Scores {
		[JsonPropertyName("1")]
		public _1 _1 { get; set; }

		[JsonPropertyName("2")]
		public _2 _2 { get; set; }

		[JsonPropertyName("3")]
		public _3 _3 { get; set; }
	}

	public class StadiumData {
		public string id { get; set; }
		public string name { get; set; }
		public string city { get; set; }
		public string country { get; set; }
		public object capacity { get; set; }
		public object googlecoords { get; set; }
	}

	public class Stats {
		public List<string> aces { get; set; }
		public List<string> double_faults { get; set; }
		public List<string> win_1st_serve { get; set; }
		public List<string> break_point_conversions { get; set; }
	}
}

public class BetsapiData_TennisMatchDetail_Upcoming {
	public int success { get; set; }
	public List<Result> results { get; set; }

	[JsonIgnore]
	public bool succeed => this.success == 1;
	[JsonIgnore]
	public bool failed => this.success != 1;

	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class Away {
		public string id { get; set; }
		public string name { get; set; }
		public int image_id { get; set; }
		public string? cc { get; set; }
	}

	public class Extra {
		public string bestofsets { get; set; }
		public StadiumData stadium_data { get; set; }
		public string round { get; set; }
		public string ground { get; set; }
	}

	public class Home {
		public string id { get; set; }
		public string name { get; set; }
		public int image_id { get; set; }
		public string? cc { get; set; }
	}

	public class League {
		public string id { get; set; }
		public string name { get; set; }
		public object cc { get; set; }
	}

	public class Result {
		public long id { get; set; }
		public string sport_id { get; set; }
		public long time { get; set; }
		public int time_status { get; set; }
		public League league { get; set; }
		public Home home { get; set; }
		public Away away { get; set; }
		public string? ss { get; set; }
		public Extra extra { get; set; }
		public string inplay_created_at { get; set; }
		public string inplay_updated_at { get; set; }
		public string confirmed_at { get; set; }
		public string bet365_id { get; set; }
	}

	public class StadiumData {
		public string id { get; set; }
		public string name { get; set; }
		public string city { get; set; }
		public string country { get; set; }
		public object capacity { get; set; }
		public object googlecoords { get; set; }
	}
}
