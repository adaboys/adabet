namespace App;

using System.Text.Json.Serialization;

public class Betsapi_UpcomingMatchesData {
	public int success { get; set; }
	public Pager pager { get; set; }
	public List<Result> results { get; set; }

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

	public class OHome {
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
		public object ss { get; set; }
		public string round { get; set; }
		public string bet365_id { get; set; }
		public OHome o_home { get; set; }
	}
}
