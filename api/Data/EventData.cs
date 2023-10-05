namespace App;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class EventResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "page_pos")]
		public int page_pos { get; set; }

		[JsonPropertyName(name: "page_count")]
		public int page_count { get; set; }

		[JsonPropertyName(name: "total_item_count")]
		public int total_item_count { get; set; }

		[JsonPropertyName(name: "applicants")]
		public Applicant[] applicants { get; set; }
	}

	public class Applicant {
		[JsonPropertyName(name: "player_name")]
		public string player_name { get; set; }

		[JsonPropertyName(name: "gift_ada_amount")]
		public decimal gift_ada_amount { get; set; }

		[JsonPropertyName(name: "gift_isky_amount")]
		public decimal gift_abe_amount { get; set; }

		[JsonPropertyName(name: "gift_delivery_status")]
		public byte gift_delivery_status { get; set; }
	}
}
