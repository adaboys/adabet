namespace App;

using System.Text.Json.Serialization;

public class BetsapiData_MergeHistory {
	public int success { get; set; }
	public Pager pager { get; set; }
	public List<Result> results { get; set; }

	[JsonIgnore]
	public bool succeed => this.success == 1;
	[JsonIgnore]
	public bool failed => this.success != 1;

	public class Pager {
		public int page { get; set; }
		public int per_page { get; set; }
		public int total { get; set; }
	}

	public class Result {
		public long from_id { get; set; }
		public long to_id { get; set; }
		public long created_at { get; set; }
	}
}
