namespace App;

using System.Text.Json.Serialization;

public class BetMetadata {
	[JsonPropertyName(name: "674")]
	public Cip674 cip_674 { get; set; }

	public class Cip674 {
		[JsonPropertyName(name: "msg")]
		public string[] msg_list { get; set; }
	}
}
