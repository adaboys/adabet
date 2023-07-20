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

		[JsonPropertyName(name: "ada")]
		public decimal ada { get; set; }

		[JsonPropertyName(name: "abe")]
		public decimal abe { get; set; }

		[JsonPropertyName(name: "gem")]
		public decimal gem { get; set; }
	}
}

public class WithdrawCoinRequestBody {
	[Required]
	[JsonPropertyName(name: "receiver_address")]
	public string receiver_address { get; set; }

	[Required]
	[JsonPropertyName(name: "currency_id")]
	public int currency_id { get; set; }

	[MinDk(0.0)]
	[JsonPropertyName(name: "amount")]
	public decimal amount { get; set; }
}

public class PerformWithdrawActualRequestBody {
	[Required]
	[JsonPropertyName(name: "otp_code")]
	public string otp_code { get; set; }
}
