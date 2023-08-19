namespace App;

using System.Text.Json.Serialization;

public class BetsapiData_MatchHistory {
	public int success { get; set; }
	public Results results { get; set; }

	[JsonIgnore]
	public bool succeed => this.success == 1;
	[JsonIgnore]
	public bool failed => this.success != 1;

	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class Away {
		public long id { get; set; }
		public string name { get; set; }
		public string image_id { get; set; }
		public string cc { get; set; }
		public string sport_id { get; set; }
		public League league { get; set; }
		public Home home { get; set; }
		public Away away { get; set; }
		public string time { get; set; }
		public string ss { get; set; }
		public string time_status { get; set; }
	}

	public class H2h {
		public string id { get; set; }
		public string sport_id { get; set; }
		public League league { get; set; }
		public Home home { get; set; }
		public Away away { get; set; }
		public string time { get; set; }
		public string ss { get; set; }
		public string time_status { get; set; }
	}

	public class Home {
		public long id { get; set; }
		public string name { get; set; }
		public string image_id { get; set; }
		public string cc { get; set; }
		public string sport_id { get; set; }
		public League league { get; set; }
		public Home home { get; set; }
		public Away away { get; set; }
		public string time { get; set; }
		public string ss { get; set; }
		public string time_status { get; set; }
	}

	public class League {
		public string id { get; set; }
		public string name { get; set; }
		public string cc { get; set; }
	}

	public class Results {
		public List<H2h> h2h { get; set; }
		public List<Home> home { get; set; }
		public List<Away> away { get; set; }
	}
}
