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
	[JsonPropertyName(name: "tmp_otp_code")]
	public string? tmp_otp_code { get; set; }

	[Required]
	[JsonPropertyName(name: "receiver_address")]
	public string receiver_address { get; set; }

	[JsonPropertyName(name: "send_all")]
	public bool send_all { get; set; } = false;

	[MinDk((double)AppConst.MIN_ADA_TO_SEND)]
	[JsonPropertyName(name: "ada_amount")]
	public decimal ada_amount { get; set; }

	[MinDk(0.0)]
	[JsonPropertyName(name: "abe_amount")]
	public decimal abe_amount { get; set; }

	[MinDk(0.0)]
	[JsonPropertyName(name: "gem_amount")]
	public decimal gem_amount { get; set; }
}

public class PerformWithdrawActualRequestBody {
	[Required]
	[JsonPropertyName(name: "otp_code")]
	public string otp_code { get; set; }
}
