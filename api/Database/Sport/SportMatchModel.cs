namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

/// Sport match. For eg,. VN - JP
/// It includes market of odds.
[Table(DbConst.table_sport_match)]
[Index(nameof(league_id), nameof(status), nameof(home_team_id), nameof(away_team_id))]
[Index(nameof(start_at), nameof(status))]
[Index(nameof(predictable), nameof(prediction_rank_calculated))]
[Index(nameof(ref_betsapi_match_id))] // Allow duplicate since event_merge in betsapi
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

	[Column("lock_mode", TypeName = "tinyint")]
	public SportMatchModelConst.LockMode lock_mode { get; set; }

	/// When true, the match can be predicted from users.
	/// After the match end, we give reward to users who has win in prediction.
	[Column("predictable")]
	public bool predictable { get; set; }

	[Column("prediction_rank_calculated")]
	public bool prediction_rank_calculated { get; set; }

	/// When the match be started in UTC.
	[Column("start_at")]
	public DateTime start_at { get; set; }

	/// Current played time in seconds. For eg,. 310 = 05:10.
	[Column("timer")]
	public short timer { get; set; }

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

/// For json conversion.
public class Market {
	/// Unique market name. For eg,. result, total, handicap, both_to_score,...
	[JsonPropertyName(name: "name")]
	public string name { get; set; }

	/// List of odd for this market.
	[JsonPropertyName(name: "odds")]
	public List<Odd> odds { get; set; }
}

/// For json conversion.
public class Odd {
	/// Odd name. For eg,. win1, draw, win2, total_2.5, handicap_-1.0, bts_yes,...
	[JsonPropertyName(name: "k")]
	public string name { get; set; }

	/// Predicted odd value. For eg,. 1.232, 9.102
	[JsonPropertyName(name: "v")]
	public decimal value { get; set; }

	[JsonPropertyName(name: "s")]
	public bool suspend { get; set; }
}

public class SportMatchModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<SportMatchModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");

		// // Ref: https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew
		// modelBuilder.Entity<SportMatchModel>().OwnsMany(m => m.markets, ownedNavigationBuilder => {
		// 	ownedNavigationBuilder.ToJson();
		// 	ownedNavigationBuilder.OwnsMany(m1 => m1.odds);
		// });
	}
}

public class SportMatchModelConst {
	public enum TimeStatus {
		Nothing = 0, // Null
		Upcoming = 1, // 予定試合
		InPlay = 2, // 放送中
		Ended = 3, // 終了（最終状態）
		ToBeFixed = 4, // what??
		Postponed = 5, // リスケ
		Cancelled = 6, // 取消（最終状態）
		Walkover = 7, // あるチームが欠席で、試合が次の状態（取消？）になる
		Interrupted = 8, // 中断
		Abandoned = 9,
		Retired = 10,
		Suspended = 11, // 一時停止
		DecidedByFa = 12, // FAにて決定議論中

		Removed = 99, // 第三者システムから削除
		RemovedSinceNotFoundInBetsapi = 100, // 第三者システムから見つからない
	}

	public enum LockMode {
		Nothing = 0, // Null
		LockBetting = 1, // Lock betting, prediction
	}

	/// Ref: https://betsapi.com/docs/GLOSSARY.html
	public static TimeStatus ConvertFromBetsapiStatus(int timeStatus) {
		return timeStatus switch {
			0 => TimeStatus.Upcoming,
			1 => TimeStatus.InPlay,
			2 => TimeStatus.ToBeFixed,
			3 => TimeStatus.Ended,
			4 => TimeStatus.Postponed,
			5 => TimeStatus.Cancelled,
			6 => TimeStatus.Walkover,
			7 => TimeStatus.Interrupted,
			8 => TimeStatus.Abandoned,
			9 => TimeStatus.Retired,
			10 => TimeStatus.Suspended,
			11 => TimeStatus.DecidedByFa,
			99 => TimeStatus.Removed,
			_ => TimeStatus.Nothing
		};
	}

	public static TimeStatus[] BettableMatchStatusList = new[] {
		TimeStatus.Upcoming,
		TimeStatus.InPlay
	};

	public static readonly TimeStatus[] ComingSoonMatchStatusList = new TimeStatus[] {
		TimeStatus.ToBeFixed,
		TimeStatus.Postponed,
		TimeStatus.Walkover,
		TimeStatus.Interrupted,
		TimeStatus.Abandoned,
		TimeStatus.Retired,
		TimeStatus.Suspended,
		TimeStatus.DecidedByFa,
	};
}

public class MarketConst {
	public const string MainFullTime = "1x2";
	public const string DoubleChance = "dbc";
	public const string BothToScore = "bts";
	public const string Totals = "tt";
	public const string AsianTotals = "att";
	public const string HomeTotals = "tt1";
	public const string AwayTotals = "tt2";
	public const string HomeAsianTotals = "att1";
	public const string AwayAsianTotals = "att2";
	public const string HomeHandicaps = "hdc1";
	public const string AwayHandicaps = "hdc2";
}

public class OddConst {
	public const string HomeWin = "1";
	public const string Draw = "x";
	public const string AwayWin = "2";

	public const string HomeOrDraw = "1x";
	public const string AwayOrDraw = "2x";
	public const string HomeOrAway = "12";

	public const string Yes = "y";
	public const string No = "n";
	public const string Over = "ov";
	public const string Under = "ud";
	public const string Odd = "od";
	public const string Handicap = "hdc";
}
