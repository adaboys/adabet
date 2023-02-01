namespace App;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class GetLinkedExternalWalletsResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "wallets")]
		public List<Wallet> wallets { get; set; }
	}

	public class Wallet {
		[JsonPropertyName(name: "name")]
		public string? name { get; set; }

		[JsonPropertyName(name: "address")]
		public string address { get; set; }

		[JsonPropertyName(name: "status")]
		public int status { get; set; }

		[JsonPropertyName(name: "ada_balance")]
		public decimal ada_balance { get; set; }

		[JsonPropertyName(name: "abe_balance")]
		public decimal abe_balance { get; set; }
	}
}
