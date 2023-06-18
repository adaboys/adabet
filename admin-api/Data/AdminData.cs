namespace App;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class GetAdminProfileResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "name")]
		public string? name { get; set; }

		[JsonPropertyName(name: "player_name")]
		public string player_name { get; set; }

		[JsonPropertyName(name: "email")]
		public string email { get; set; }

		[JsonPropertyName(name: "code")]
		public string? code { get; set; }

		[JsonPropertyName(name: "telno")]
		public string? telno { get; set; }

		[JsonPropertyName(name: "wallet_address")]
		public string wallet_address { get; set; }

		[JsonPropertyName(name: "has_password")]
		public bool has_password { get; set; }

		[JsonPropertyName(name: "avatar")]
		public string? avatar { get; set; }
	}
}
