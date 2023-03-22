namespace App;

public class SportMatchModelConst {
	public enum TimeStatus {
		Nothing = 0, // Null
		Upcoming = 1, // Not yet start (in schedule)
		InPlay = 2, // Playing (live), match in progress
		Ended = 3, // The match ended
		ToBeFixed = 4, // what??
		Postponed = 5, // Wait a while until open again
		Cancelled = 6, // The match was cancelled
		Walkover = 7, // Win, for eg,. away absent
		Interrupted = 8, // Something wrong with internet connection??
		Abandoned = 9,
		Retired = 10,
		Suspended = 11,
		DecidedByFa = 12,
		Removed = 13,
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

	public static readonly TimeStatus[] PausingMatchStatusList = new TimeStatus[] {
		TimeStatus.Suspended,
		TimeStatus.Postponed
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
