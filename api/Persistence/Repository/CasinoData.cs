namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Casino_RegisterUserRequestBody {
	[JsonPropertyName(name: "user_id")]
	public Guid user_id { get; set; }

	[JsonPropertyName(name: "name")]
	public string? name { get; set; }

	[JsonPropertyName(name: "email")]
	public string? email { get; set; }

	[JsonPropertyName(name: "password")]
	public string? password { get; set; }

	[JsonPropertyName(name: "wallet")]
	public string wallet { get; set; }
}

public class Casino_RegisterUserResponse : ApiResponse {
	[JsonPropertyName(name: "data")]
	public object? data { get; set; }
}
