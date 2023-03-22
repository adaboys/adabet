namespace App;

using System.Text.Json.Serialization;

public class AppResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public AppData data { get; set; }

	public class AppData {
		[JsonPropertyName(name: "os_type")]
		public int os_type { get; set; }

		[JsonPropertyName(name: "app_version")]
		public string app_version { get; set; }
	}
}
