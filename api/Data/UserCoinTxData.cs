namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class GetCoinTxHistoryResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "page_pos")]
		public int page_pos { get; set; }

		[JsonPropertyName(name: "page_count")]
		public int page_count { get; set; }

		[JsonPropertyName(name: "total_item_count")]
		public int total_item_count { get; set; }

		[JsonPropertyName(name: "histories")]
		public History[] histories { get; set; }
	}

	public class History {
		[JsonPropertyName(name: "forward_currency_id")]
		public int forward_currency_id { get; set; }
	}
}
