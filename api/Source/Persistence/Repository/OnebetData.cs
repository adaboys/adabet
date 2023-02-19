namespace App;

// Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
public class Onebet_DataMatch {
	public int v { get; set; }
	public int id { get; set; }
	public string team1 { get; set; }
	public string team1_rus { get; set; }
	public int team1_id { get; set; }
	public string team2 { get; set; }
	public string team2_rus { get; set; }
	public int team2_id { get; set; }
	public short score1 { get; set; }
	public short score2 { get; set; }
	public string href { get; set; }
	public DateTime date_start { get; set; }
	public bool isLive { get; set; }
	public bool isComposite { get; set; }
	public bool isSpecial { get; set; }
	public short minute { get; set; }
	public short seconds { get; set; }
	public int half_order_index { get; set; }
	public string title { get; set; }
	public string country { get; set; }
	public League league { get; set; }
	public Markets? markets { get; set; }
	public string hash { get; set; }
	public DateTime actual_at { get; set; }
	public Stats? stats { get; set; }

	public class BothToScore {
		public Yes? yes { get; set; }
		public No? no { get; set; }
	}

	public class Half {
	}

	public class Handicaps1 {
		public decimal type { get; set; }
		public decimal v { get; set; }
	}

	public class Handicaps2 {
		public decimal type { get; set; }
		public decimal v { get; set; }
	}

	public class League {
		public int country_id { get; set; }
		public int league_id { get; set; }
		public int sport_id { get; set; }
		public string name { get; set; }
		public string name_rus { get; set; }
		public bool isCyber { get; set; }
		public bool isSimulated { get; set; }
		public bool isSpecial { get; set; }
	}

	public class Markets {
		public List<Total>? totals { get; set; }
		public List<Totals1>? totals1 { get; set; }
		public List<Totals2>? totals2 { get; set; }
		public List<TotalsAsian>? totalsAsian { get; set; }
		public List<Totals1Asian>? totals1Asian { get; set; }
		public List<Totals2Asian>? totals2Asian { get; set; }
		public List<Handicaps1>? handicaps1 { get; set; }
		public List<Handicaps2>? handicaps2 { get; set; }
		public BothToScore? bothToScore { get; set; }
		public Half? half { get; set; }
		public Win1? win1 { get; set; }
		public WinX? winX { get; set; }
		public Win2? win2 { get; set; }
		public Win1X win1X { get; set; }
		public Win12 win12 { get; set; }
		public WinX2 winX2 { get; set; }
	}

	public class No {
		public decimal v { get; set; }
	}

	public class Over {
		public decimal v { get; set; }
	}

	public class Stats {
		public int corners1 { get; set; }
		public int yellow_cards1 { get; set; }
		public int red_cards1 { get; set; }
		public int subs1 { get; set; }
		public int shoots_on1 { get; set; }
		public int shoots_off1 { get; set; }
		public int attacks1 { get; set; }
		public int attacks_danger_1 { get; set; }
		public int corners2 { get; set; }
		public int yellow_cards2 { get; set; }
		public int red_cards2 { get; set; }
		public int subs2 { get; set; }
		public int shoots_on2 { get; set; }
		public int shoots_off2 { get; set; }
		public int attacks2 { get; set; }
		public int attacks_danger_2 { get; set; }
		public int free_kicks1 { get; set; }
		public int free_kicks2 { get; set; }
	}

	public class Total {
		public decimal type { get; set; }
		public Over over { get; set; }
		public Under under { get; set; }
	}

	public class Totals1 {
		public decimal type { get; set; }
		public Over over { get; set; }
		public Under under { get; set; }
	}

	public class Totals1Asian {
		public decimal type { get; set; }
		public Over over { get; set; }
		public Under under { get; set; }
	}

	public class Totals2 {
		public decimal type { get; set; }
		public Over over { get; set; }
		public Under under { get; set; }
	}

	public class Totals2Asian {
		public decimal type { get; set; }
		public Over over { get; set; }
		public Under under { get; set; }
	}

	public class TotalsAsian {
		public decimal type { get; set; }
		public Over over { get; set; }
		public Under under { get; set; }
	}

	public class Under {
		public decimal v { get; set; }
	}

	public class Win1 {
		public decimal v { get; set; }
	}

	public class Win12 {
		public decimal v { get; set; }
	}

	public class Win1X {
		public decimal v { get; set; }
	}

	public class Win2 {
		public decimal v { get; set; }
	}

	public class WinX {
		public decimal v { get; set; }
	}

	public class WinX2 {
		public decimal v { get; set; }
	}

	public class Yes {
		public decimal v { get; set; }
	}
}
