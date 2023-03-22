namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

/// Sport match. For eg,. VN - JP
/// It includes market of odds.
[Table(DbConst.table_sport_match)]
[Index(nameof(league_id), nameof(status), nameof(home_team_id), nameof(away_team_id))]
[Index(nameof(start_at))]
[Index(nameof(ref_betsapi_match_id), IsUnique = true)]
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

	/// When true, the match can be freely betted from users.
	/// After the match end, we give reward to users who has win in prediction.
	[Column("user_can_predict_free")]
	public bool user_can_predict_free { get; set; }

	/// When the match be started in UTC.
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

	/// Since Json type requires fixed datastructure, and each instance inside it is tracked.
	/// -> it is a bit of hard when we want to replace with new data in Json.
	/// -> so we use string instead, and decode to list of market when query.
	/// Json type for store current odds of the match. At this time, it does work with Dictionary type.
	/// In general, this object will be updated everytime we fetch data from 3rd sites.
	/// TechNote: Default type of Json is nvarchar(MAX).
	// For Json type: public List<Market> markets { get; set; }
	[Column("markets", TypeName = "varchar(MAX)")]
	public string? markets { get; set; }

	/// Hold current number of bet that made by user on this match.
	/// This just increment 1 each time user places a bet, and ignores number of market/odd in the bet.
	[Column("user_bet_count")]
	public int user_bet_count { get; set; }

	/// Reference of parent match id at betsapi.
	/// Since betsapi performs merge matches, so we store to_match_id as parent
	/// to make relationship of matches in our system.
	[Column("ref_betsapi_parent_match_id")]
	public long ref_betsapi_parent_match_id { get; set; }

	/// Reference of match id at betsapi
	[Column("ref_betsapi_match_id")]
	public long ref_betsapi_match_id { get; set; }

	/// Reference of home team id at betsapi
	[Column("ref_betsapi_home_team_id")]
	public long ref_betsapi_home_team_id { get; set; }

	/// Reference of away team id at betsapi
	[Column("ref_betsapi_away_team_id")]
	public long ref_betsapi_away_team_id { get; set; }

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

	[JsonPropertyName(name: "s")]
	public bool suspend { get; set; }
}
