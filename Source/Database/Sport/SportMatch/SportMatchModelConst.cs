namespace App;

public class SportMatchModelConst {
	public enum TimeStatus {
		NotYetStarted = 1, // Not yet started
		InPlay = 2, // Playing (live), match in progress
		ToBeFixed = 3, // what??
		Ended = 4, // The match ended
		Postponed = 5, // Wait a while until open
		Cancelled = 6, // Be cancelled
		Walkover = 7, // Win, for eg,. away absent
		Interrupted = 8, // Something wrong with internet connection??
		Abandoned = 9,
		Retired = 10,
		Suspended = 11,
		DecidedByFa = 12,
		Removed = 99,
	}

	/// Ref: https://betsapi.com/docs/GLOSSARY.html
	public TimeStatus ConvertBetsapiStatus(int timeStatus) {
		return timeStatus switch {
			0 => TimeStatus.NotYetStarted,
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
			_ => TimeStatus.NotYetStarted
		};
	}

	public static TimeStatus[] BETTABLE_MATCH_STATUS_LIST = new[] {
		TimeStatus.NotYetStarted,
		TimeStatus.InPlay
	};
}

public class MarketConst {
	public const string MainFullTime = "1x2";
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
	public const string HomeWin = "w1";
	public const string Draw = "x";
	public const string AwayWin = "w2";
	public const string Yes = "y";
	public const string No = "n";
	public const string Over = "ov";
	public const string Under = "ud";
	public const string Odd = "od";
	public const string Handicap = "hdc";
}
