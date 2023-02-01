namespace App;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class GenApiKeyRequestBody {
	[Required]
	[JsonPropertyName(name: "size")]
	public int size { get; set; }

	/// API key should contain some special chars
	[Required]
	[JsonPropertyName(name: "special_chars")]
	public string special_chars { get; set; }

	/// Minimum special chars that API key must contain.
	[Required]
	[JsonPropertyName(name: "min_special_char_count")]
	public int min_special_char_count { get; set; }

	/// API key must contain all this chars.
	[JsonPropertyName(name: "required_chars")]
	public string required_chars { get; set; }
}
