namespace App;

using System.Text.Json.Serialization;

public class Betsapi_InplayMatchesData {
	public int success { get; set; }
	public Pager pager { get; set; }
	public List<Result> results { get; set; }

	public class _1 {
		public string home { get; set; }
		public string away { get; set; }
	}

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

	public class Home {
		public long id { get; set; }
		public string name { get; set; }
		public string image_id { get; set; }
		public string cc { get; set; }
	}

	public class League {
		public long id { get; set; }
		public string name { get; set; }
		public string cc { get; set; }
	}

	public class OAway {
		public string id { get; set; }
		public string name { get; set; }
		public string image_id { get; set; }
		public object cc { get; set; }
	}

	public class Pager {
		public int page { get; set; }
		public int per_page { get; set; }
		public int total { get; set; }
	}

	public class Result {
		public long id { get; set; }
		public string sport_id { get; set; }
		public long time { get; set; }
		public string time_status { get; set; }
		public League league { get; set; }
		public Home home { get; set; }
		public Away away { get; set; }
		public string ss { get; set; }
		public Scores? scores { get; set; }
		public string bet365_id { get; set; }
		public Timer? timer { get; set; }
		public Stats? stats { get; set; }
		public OAway? o_away { get; set; }
	}

	public class Scores {
		[JsonPropertyName("2")]
		public _2 _2 { get; set; }

		[JsonPropertyName("1")]
		public _1 _1 { get; set; }
	}

	public class Stats {
		public List<string> action_areas { get; set; }
		public List<string> attacks { get; set; }
		public List<string> corners { get; set; }
		public List<string> corner_f { get; set; }
		public List<string> corner_h { get; set; }
		public List<string> crosses { get; set; }
		public List<string> crossing_accuracy { get; set; }
		public List<string> dangerous_attacks { get; set; }
		public List<string> goals { get; set; }
		public List<string> key_passes { get; set; }
		public List<string> off_target { get; set; }
		public List<string> on_target { get; set; }
		public List<string> passing_accuracy { get; set; }
		public List<string> penalties { get; set; }
		public List<string> possession_rt { get; set; }
		public List<string> redcards { get; set; }
		public List<string> substitutions { get; set; }
		public List<string> xg { get; set; }
		public List<string> yellowcards { get; set; }
		public List<string> ball_safe { get; set; }
		public List<string> yellowred_cards { get; set; }
		public List<string> fouls { get; set; }
		public List<string> freekicks { get; set; }
		public List<string> goalattempts { get; set; }
		public List<string> goalkicks { get; set; }
		public List<string> throwins { get; set; }
	}

	public class Timer {
		public int tm { get; set; }
		public int ts { get; set; }
		public string tt { get; set; }
		public int ta { get; set; }
		public int md { get; set; }
	}
}