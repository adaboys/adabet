namespace App;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class ValidateTokenResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "expires_in")]
		public int expires_in { get; set; }
	}
}
