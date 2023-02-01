namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

/// Sport match. For eg,. VN - JP
/// It includes market of odds.
[Table(DbConst.table_sport_match)]
[Index(nameof(league_id), nameof(status), nameof(home_team_id), nameof(away_team_id))]
[Index(nameof(ref_betsapi_home_team_id), nameof(ref_betsapi_away_team_id))]
public class SportMatchModel : AutoGenerateUpdateTime {
	[Key]
	[Column("id")]
	public long id { get; set; }

	/// Which sport (soccer, tennis,...)
	[ForeignKey(DbConst.table_sport_league)]
	[Column("league_id")]
	public long league_id { get; set; }

	/// FK to [ForeignKey(DbConst.table_sport_team)]
	[Column("home_team_id")]
	public long home_team_id { get; set; }

	/// FK to [ForeignKey(DbConst.table_mst_sport_team)]
	[Column("away_team_id")]
	public long away_team_id { get; set; }

	/// [See const] Match status (not yet started, inplay, ended,...)
	[Required]
	[Column("status", TypeName = "tinyint")]
	public SportMatchModelConst.TimeStatus status { get; set; }

	/// When the match be started.
	[Column("start_at")]
	public DateTime start_at { get; set; }

	/// Current played time in seconds. For eg,. 310 = 05:10.
	[Column("cur_play_time")]
	public short cur_play_time { get; set; }

	/// Current (latest) score of home team.
	[Column("home_score")]
	public short home_score { get; set; }

	/// Current (latest) score of away team.
	[Column("away_score")]
	public short away_score { get; set; }

	/// Halftime (closed) score of home team.
	[Column("home_halftime_score")]
	public short home_halftime_score { get; set; }

	/// Halftime (closed) score of away team.
	[Column("away_halftime_score")]
	public short away_halftime_score { get; set; }

	/// Fulltime (closed) score of home team.
	[Column("home_fulltime_score")]
	public short home_fulltime_score { get; set; }

	/// Fulltime (closed) score of away team.
	[Column("away_fulltime_score")]
	public short away_fulltime_score { get; set; }

	/// Json type for store current odds of the match. At this time, it does work with Dictionary type.
	/// In general, this object will be updated everytime we fetch data from 3rd sites.
	/// TechNote: Default type of Json is nvarchar(MAX).
	[Required]
	[Column("markets")]
	public List<Market> markets { get; set; }

	/// Hold current number of bet that made by user on this match.
	/// This just increment 1 each time user places a bet, and ignores number of market/odd in the bet.
	[Column("user_bet_count")]
	public int user_bet_count { get; set; }

	/// Reference of home team id from 1xbet api
	[Column("ref_onebet_home_team_id")]
	public long ref_onebet_home_team_id { get; set; }

	/// Reference of away team id from 1xbet api
	[Column("ref_onebet_away_team_id")]
	public long ref_onebet_away_team_id { get; set; }

	/// Reference of match id from betsapi
	[Column("ref_betsapi_match_id", TypeName = "varchar(64)"), MaxLength(64)]
	public string ref_betsapi_match_id { get; set; }

	/// Reference of away team id from betsapi
	[Column("ref_betsapi_home_team_id", TypeName = "varchar(64)"), MaxLength(64)]
	public string ref_betsapi_home_team_id { get; set; }

	/// Reference of away team id from betsapi
	[Column("ref_betsapi_away_team_id", TypeName = "varchar(64)"), MaxLength(64)]
	public string ref_betsapi_away_team_id { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }

	/// FK models (property name must be same with table name)
	public SportLeagueModel sport_league { get; set; }
}

public class Market {
	/// Unique market name. For eg,. result, total, handicap, both_to_score,...
	[JsonPropertyName(name: "name")]
	public string name { get; set; }

	/// List of odd for this market.
	[JsonPropertyName(name: "odds")]
	public List<Odd> odds { get; set; }
}

public class Odd {
	/// Odd name. For eg,. win1, draw, win2, total_2.5, handicap_-1.0, bts_yes,...
	[JsonPropertyName(name: "k")]
	public string name { get; set; }

	/// Predicted odd value. For eg,. 1.232, 9.102
	[JsonPropertyName(name: "v")]
	[Precision(4, 2)]
	public decimal value { get; set; }
}
