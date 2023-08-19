namespace App;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class GetCurrencyListResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "currencies")]
		public Currency[] currencies { get; set; }
	}

	public class Currency {
		public string tmpCode;
		public int tmpDecimal;

		[JsonPropertyName(name: "id")]
		public int id { get; set; }

		[JsonPropertyName(name: "name")]
		public string name { get; set; }

		[JsonPropertyName(name: "limit_bet")]
		public decimal limit_bet { get; set; }

		[JsonPropertyName(name: "amount")]
		public decimal amount { get; set; }
	}
}
