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
		public string id { get; set; }
		public string name { get; set; }
		public string image_id { get; set; }
		public string cc { get; set; }
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
		public string id { get; set; }
		public string sport_id { get; set; }
		public string time { get; set; }
		public string time_status { get; set; }
		public League league { get; set; }
		public Home home { get; set; }
		public Away away { get; set; }
		public string ss { get; set; }
		public Scores scores { get; set; }
		public string bet365_id { get; set; }
		public Timer timer { get; set; }
		public Stats stats { get; set; }
		public OAway o_away { get; set; }
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


public class Betsapi_OddsSummaryData {
	public int success { get; set; }
	public Results results { get; set; }

	[JsonIgnore]
	public bool succeed => this.success == 1;
	[JsonIgnore]
	public bool failed => this.success != 1;

	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class _10Bet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class _11 {
		public string id { get; set; }
		public string home_od { get; set; }
		public string draw_od { get; set; }
		public string away_od { get; set; }
		public object ss { get; set; }
		public object time_str { get; set; }
		public string add_time { get; set; }
	}

	public class _12 {
		public string id { get; set; }
		public string home_od { get; set; }
		public string handicap { get; set; }
		public string away_od { get; set; }
		public object ss { get; set; }
		public object time_str { get; set; }
		public string add_time { get; set; }
	}

	public class _13 {
		public string id { get; set; }
		public string over_od { get; set; }
		public string handicap { get; set; }
		public string under_od { get; set; }
		public object ss { get; set; }
		public object time_str { get; set; }
		public string add_time { get; set; }
	}

	public class _188Bet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class _1XBet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class _888Sport {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Bet365 {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class BetAtHome {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class BetClic {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class BetFair {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class BetRegal {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Betsson {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class BetVictor {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class BWin {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class CashPoint {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class CloudBet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Coral {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class DafaBet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class End {
		[JsonPropertyName("1_1")]
		public _11 _1_1 { get; set; }

		[JsonPropertyName("1_2")]
		public _12 _1_2 { get; set; }

		[JsonPropertyName("1_3")]
		public _13 _1_3 { get; set; }
	}

	public class GGBet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Intertops {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Interwetten {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Kickoff {
		[JsonPropertyName("1_1")]
		public _11 _1_1 { get; set; }

		[JsonPropertyName("1_2")]
		public _12 _1_2 { get; set; }

		[JsonPropertyName("1_3")]
		public _13 _1_3 { get; set; }
	}

	public class Ladbrokes {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Macauslot {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Mansion {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class MelBet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class NitrogenSports {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Odds {
		public Start start { get; set; }
		public Kickoff kickoff { get; set; }
		public End end { get; set; }
	}

	public class OddsUpdate {
		[JsonPropertyName("1_1")]
		public int _1_1 { get; set; }

		[JsonPropertyName("1_2")]
		public int _1_2 { get; set; }

		[JsonPropertyName("1_3")]
		public int _1_3 { get; set; }

		[JsonPropertyName("1_4")]
		public int _1_4 { get; set; }

		[JsonPropertyName("1_5")]
		public int _1_5 { get; set; }

		[JsonPropertyName("1_6")]
		public int _1_6 { get; set; }

		[JsonPropertyName("1_7")]
		public int _1_7 { get; set; }

		[JsonPropertyName("1_8")]
		public int _1_8 { get; set; }
	}

	public class PinnacleSports {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Results {
		public Bet365 Bet365 { get; set; }

		[JsonPropertyName("10Bet")]
		public _10Bet _10Bet { get; set; }
		public Ladbrokes Ladbrokes { get; set; }
		public YSB88 YSB88 { get; set; }
		public WilliamHill WilliamHill { get; set; }
		public BetClic BetClic { get; set; }
		public PinnacleSports PinnacleSports { get; set; }

		[JsonPropertyName("188Bet")]
		public _188Bet _188Bet { get; set; }
		public UniBet UniBet { get; set; }
		public BWin BWin { get; set; }
		public BetFair BetFair { get; set; }
		public CloudBet CloudBet { get; set; }
		public Betsson Betsson { get; set; }
		public SBOBET SBOBET { get; set; }

		[JsonPropertyName("888Sport")]
		public _888Sport _888Sport { get; set; }
		public TitanBet TitanBet { get; set; }
		public BetAtHome BetAtHome { get; set; }
		public DafaBet DafaBet { get; set; }
		public BetVictor BetVictor { get; set; }
		public Intertops Intertops { get; set; }
		public Interwetten Interwetten { get; set; }

		[JsonPropertyName("1XBet")]
		public _1XBet _1XBet { get; set; }
		public NitrogenSports NitrogenSports { get; set; }
		public BetRegal BetRegal { get; set; }
		public Mansion Mansion { get; set; }
		public CashPoint CashPoint { get; set; }
		public MelBet MelBet { get; set; }
		public Coral Coral { get; set; }
		public Macauslot Macauslot { get; set; }
		public GGBet GGBet { get; set; }
	}

	public class SBOBET {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Start {
		[JsonPropertyName("1_1")]
		public _11 _1_1 { get; set; }

		[JsonPropertyName("1_2")]
		public _12 _1_2 { get; set; }

		[JsonPropertyName("1_3")]
		public _13 _1_3 { get; set; }
	}

	public class TitanBet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class UniBet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class WilliamHill {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class YSB88 {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}
}
