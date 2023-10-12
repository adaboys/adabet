namespace App;

using System.Text.Json.Serialization;

public class Betsapi_SoccerOddsSummaryData {
	public int success { get; set; }
	public Results results { get; set; }

	[JsonIgnore]
	public bool succeed => this.success == 1;
	[JsonIgnore]
	public bool failed => this.success != 1;

	public class _10Bet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class _11 {
		public string id { get; set; }
		public string? home_od { get; set; }
		public string? draw_od { get; set; }
		public string? away_od { get; set; }
		public string? ss { get; set; }
		public string? time_str { get; set; }
		public long add_time { get; set; }
	}

	public class _12 {
		public string id { get; set; }
		public string? home_od { get; set; }
		public string? handicap { get; set; }
		public string? away_od { get; set; }
		public string? ss { get; set; }
		public string? time_str { get; set; }
		public long add_time { get; set; }
	}

	public class _13 {
		public string id { get; set; }
		public string? over_od { get; set; }
		public string? handicap { get; set; }
		public string? under_od { get; set; }
		public string? ss { get; set; }
		public string? time_str { get; set; }
		public long add_time { get; set; }
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

	public class DafaBet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class End {
		[JsonPropertyName("1_1")]
		public _11? _1_1 { get; set; }

		[JsonPropertyName("1_2")]
		public _12? _1_2 { get; set; }

		[JsonPropertyName("1_3")]
		public _13? _1_3 { get; set; }
	}

	public class GGBet {
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
		public _11? _1_1 { get; set; }

		[JsonPropertyName("1_2")]
		public _12? _1_2 { get; set; }

		[JsonPropertyName("1_3")]
		public _13? _1_3 { get; set; }
	}

	public class Mansion {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Marathonbet {
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
		public long _1_1 { get; set; }

		[JsonPropertyName("1_2")]
		public long _1_2 { get; set; }

		[JsonPropertyName("1_3")]
		public long _1_3 { get; set; }

		[JsonPropertyName("1_4")]
		public long _1_4 { get; set; }

		[JsonPropertyName("1_5")]
		public long _1_5 { get; set; }

		[JsonPropertyName("1_6")]
		public long _1_6 { get; set; }

		[JsonPropertyName("1_7")]
		public long _1_7 { get; set; }

		[JsonPropertyName("1_8")]
		public long _1_8 { get; set; }
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

		[JsonPropertyName("888Sport")]
		public _888Sport _888Sport { get; set; }
		public BetAtHome BetAtHome { get; set; }
		public DafaBet DafaBet { get; set; }
		public Marathonbet Marathonbet { get; set; }
		public BetVictor BetVictor { get; set; }
		public Interwetten Interwetten { get; set; }

		[JsonPropertyName("1XBet")]
		public _1XBet _1XBet { get; set; }
		public NitrogenSports NitrogenSports { get; set; }
		public Mansion Mansion { get; set; }
		public CashPoint CashPoint { get; set; }
		public MelBet MelBet { get; set; }
		public GGBet GGBet { get; set; }
	}

	public class Start {
		[JsonPropertyName("1_1")]
		public _11? _1_1 { get; set; }

		[JsonPropertyName("1_2")]
		public _12? _1_2 { get; set; }

		[JsonPropertyName("1_3")]
		public _13? _1_3 { get; set; }
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


public class Betsapi_TennisOddsSummaryData {
	public int success { get; set; }
	public Results results { get; set; }

	[JsonIgnore]
	public bool succeed => this.success == 1;
	[JsonIgnore]
	public bool failed => this.success != 1;

	public class _131 {
		public string id { get; set; }
		public string home_od { get; set; }
		public string away_od { get; set; }
		public string? ss { get; set; }
		public long add_time { get; set; }
	}

	public class Bet365 {
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

	public class DafaBet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class End {
		[JsonPropertyName("13_1")]
		public _131 _13_1 { get; set; }
	}

	public class Interwetten {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Kickoff {
		[JsonPropertyName("13_1")]
		public _131 _13_1 { get; set; }
	}

	public class Odds {
		public Start start { get; set; }
		public Kickoff kickoff { get; set; }
		public End end { get; set; }
	}

	public class OddsUpdate {
		[JsonPropertyName("13_1")]
		public int _13_1 { get; set; }

		[JsonPropertyName("13_4")]
		public int _13_4 { get; set; }
	}

	public class PlanetWin365 {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Results {
		public Bet365 Bet365 { get; set; }
		public BetClic BetClic { get; set; }
		public PlanetWin365 PlanetWin365 { get; set; }
		public BWin BWin { get; set; }
		public BetFair BetFair { get; set; }
		public DafaBet DafaBet { get; set; }
		public Interwetten Interwetten { get; set; }
		public CashPoint CashPoint { get; set; }
	}

	public class Start {
		[JsonPropertyName("13_1")]
		public _131 _13_1 { get; set; }
	}
}

public class Betsapi_PingPongOddsSummaryData {
	public int success { get; set; }
	public Results results { get; set; }

	[JsonIgnore]
	public bool succeed => this.success == 1;
	[JsonIgnore]
	public bool failed => this.success != 1;

	public class _10Bet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class _921 {
		public string id { get; set; }
		public string home_od { get; set; }
		public string away_od { get; set; }
		public object ss { get; set; }
		public string add_time { get; set; }
	}

	public class _922 {
		public string id { get; set; }
		public string home_od { get; set; }
		public string handicap { get; set; }
		public string away_od { get; set; }
		public object ss { get; set; }
		public string add_time { get; set; }
	}

	public class _923 {
		public string id { get; set; }
		public string over_od { get; set; }
		public string handicap { get; set; }
		public string under_od { get; set; }
		public object ss { get; set; }
		public string add_time { get; set; }
	}

	public class Bet365 {
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

	public class DafaBet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class End {
		[JsonPropertyName("92_1")]
		public _921 _92_1 { get; set; }

		[JsonPropertyName("92_2")]
		public _922 _92_2 { get; set; }

		[JsonPropertyName("92_3")]
		public _923 _92_3 { get; set; }
	}

	public class Everygame {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Kickoff {
		[JsonPropertyName("92_1")]
		public _921 _92_1 { get; set; }

		[JsonPropertyName("92_2")]
		public _922 _92_2 { get; set; }

		[JsonPropertyName("92_3")]
		public _923 _92_3 { get; set; }
	}

	public class Odds {
		public Start start { get; set; }
		public Kickoff kickoff { get; set; }
		public End end { get; set; }
	}

	public class OddsUpdate {
		[JsonPropertyName("92_1")]
		public int _92_1 { get; set; }

		[JsonPropertyName("92_2")]
		public int _92_2 { get; set; }

		[JsonPropertyName("92_3")]
		public int _92_3 { get; set; }
	}

	public class Results {
		public Bet365 Bet365 { get; set; }

		[JsonPropertyName("10Bet")]
		public _10Bet _10Bet { get; set; }
		public BWin BWin { get; set; }
		public DafaBet DafaBet { get; set; }
		public Everygame Everygame { get; set; }
		public CashPoint CashPoint { get; set; }
	}

	public class Start {
		[JsonPropertyName("92_1")]
		public _921 _92_1 { get; set; }

		[JsonPropertyName("92_2")]
		public _922 _92_2 { get; set; }

		[JsonPropertyName("92_3")]
		public _923 _92_3 { get; set; }
	}
}

public class Betsapi_BasketballOddsSummaryData {
	public int success { get; set; }
	public Results results { get; set; }

	[JsonIgnore]
	public bool succeed => this.success == 1;
	[JsonIgnore]
	public bool failed => this.success != 1;

	public class _181 {
		public string id { get; set; }
		public string home_od { get; set; }
		public string away_od { get; set; }
		public object ss { get; set; }
		public object time_str { get; set; }
		public string add_time { get; set; }
	}

	public class _182 {
		public string id { get; set; }
		public string home_od { get; set; }
		public string handicap { get; set; }
		public string away_od { get; set; }
		public object ss { get; set; }
		public object time_str { get; set; }
		public string add_time { get; set; }
	}

	public class _183 {
		public string id { get; set; }
		public string over_od { get; set; }
		public string handicap { get; set; }
		public string under_od { get; set; }
		public object ss { get; set; }
		public object time_str { get; set; }
		public string add_time { get; set; }
	}

	public class _1XBet {
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

	public class End {
		[JsonPropertyName("18_2")]
		public _182 _18_2 { get; set; }

		[JsonPropertyName("18_3")]
		public _183 _18_3 { get; set; }

		[JsonPropertyName("18_1")]
		public _181 _18_1 { get; set; }
	}

	public class Kickoff {
		[JsonPropertyName("18_2")]
		public _182 _18_2 { get; set; }

		[JsonPropertyName("18_3")]
		public _183 _18_3 { get; set; }

		[JsonPropertyName("18_1")]
		public _181 _18_1 { get; set; }
	}

	public class Odds {
		public Start start { get; set; }
		public Kickoff kickoff { get; set; }
		public End end { get; set; }
	}

	public class OddsUpdate {
		[JsonPropertyName("18_1")]
		public int _18_1 { get; set; }

		[JsonPropertyName("18_2")]
		public int _18_2 { get; set; }

		[JsonPropertyName("18_3")]
		public int _18_3 { get; set; }

		[JsonPropertyName("18_4")]
		public int _18_4 { get; set; }

		[JsonPropertyName("18_5")]
		public int _18_5 { get; set; }

		[JsonPropertyName("18_6")]
		public int _18_6 { get; set; }

		[JsonPropertyName("18_7")]
		public int _18_7 { get; set; }

		[JsonPropertyName("18_8")]
		public int _18_8 { get; set; }

		[JsonPropertyName("18_9")]
		public int _18_9 { get; set; }
	}

	public class Results {
		public Bet365 Bet365 { get; set; }
		public BetAtHome BetAtHome { get; set; }

		[JsonPropertyName("1XBet")]
		public _1XBet _1XBet { get; set; }
	}

	public class Start {
		[JsonPropertyName("18_2")]
		public _182 _18_2 { get; set; }

		[JsonPropertyName("18_3")]
		public _183 _18_3 { get; set; }

		[JsonPropertyName("18_1")]
		public _181 _18_1 { get; set; }
	}
}

public class Betsapi_VolleyballOddsSummaryData {
	public int success { get; set; }
	public Results results { get; set; }

	[JsonIgnore]
	public bool succeed => this.success == 1;
	[JsonIgnore]
	public bool failed => this.success != 1;

	public class _1XBet {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class _911 {
		public string id { get; set; }
		public string home_od { get; set; }
		public string away_od { get; set; }
		public object ss { get; set; }
		public string add_time { get; set; }
	}

	public class _912 {
		public string id { get; set; }
		public string home_od { get; set; }
		public string handicap { get; set; }
		public string away_od { get; set; }
		public object ss { get; set; }
		public string add_time { get; set; }
	}

	public class _913 {
		public string id { get; set; }
		public string over_od { get; set; }
		public string handicap { get; set; }
		public string under_od { get; set; }
		public object ss { get; set; }
		public string add_time { get; set; }
	}

	public class Bet365 {
		public int matching_dir { get; set; }
		public OddsUpdate odds_update { get; set; }
		public Odds odds { get; set; }
	}

	public class Betsson {
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
		[JsonPropertyName("91_1")]
		public _911 _91_1 { get; set; }

		[JsonPropertyName("91_2")]
		public _912 _91_2 { get; set; }

		[JsonPropertyName("91_3")]
		public _913 _91_3 { get; set; }
	}

	public class Kickoff {
		[JsonPropertyName("91_1")]
		public _911 _91_1 { get; set; }

		[JsonPropertyName("91_2")]
		public _912 _91_2 { get; set; }

		[JsonPropertyName("91_3")]
		public _913 _91_3 { get; set; }
	}

	public class Odds {
		public Start start { get; set; }
		public Kickoff kickoff { get; set; }
		public End end { get; set; }
	}

	public class OddsUpdate {
		[JsonPropertyName("91_1")]
		public int _91_1 { get; set; }

		[JsonPropertyName("91_2")]
		public int _91_2 { get; set; }

		[JsonPropertyName("91_3")]
		public int _91_3 { get; set; }
	}

	public class Results {
		public Bet365 Bet365 { get; set; }
		public Betsson Betsson { get; set; }
		public DafaBet DafaBet { get; set; }

		[JsonPropertyName("1XBet")]
		public _1XBet _1XBet { get; set; }
	}

	public class Start {
		[JsonPropertyName("91_1")]
		public _911 _91_1 { get; set; }

		[JsonPropertyName("91_2")]
		public _912 _91_2 { get; set; }

		[JsonPropertyName("91_3")]
		public _913 _91_3 { get; set; }
	}
}
