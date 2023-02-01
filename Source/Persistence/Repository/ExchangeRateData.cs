namespace App;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class ExchangeRateResponse {
	// "time": "2022-07-15T00:52:13.0000000Z",
	[JsonPropertyName(name: "time")]
	public string time { get; set; }

	// "asset_id_base": "ADA",
	[JsonPropertyName(name: "asset_id_base")]
	public string asset_id_base { get; set; }

	// "asset_id_quote": "USD",
	[JsonPropertyName(name: "asset_id_quote")]
	public string asset_id_quote { get; set; }

	// "rate": 0.4388227901872101852343968433
	[JsonPropertyName(name: "rate")]
	public double rate { get; set; }
}
