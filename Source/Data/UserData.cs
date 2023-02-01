namespace App;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class AttemptRegisterUserRequestBody {
	[Required]
	[JsonPropertyName(name: "name")]
	public string name { get; set; }

	[Required]
	[EmailAddress]
	[JsonPropertyName(name: "email")]
	public string email { get; set; }

	[Required]
	[JsonPropertyName(name: "password")]
	public string password { get; set; }
}

public class ConfirmRegisterUserRequestBody {
	[Required]
	[EmailAddress]
	[JsonPropertyName(name: "email")]
	public string email { get; set; }

	[Required]
	[JsonPropertyName(name: "otp_code")]
	public string otp_code { get; set; }
}

public class UpdateUserIdentityRequestBody {
	[Required]
	[JsonPropertyName(name: "full_name")]
	public string full_name { get; set; }

	[Required]
	[JsonPropertyName(name: "telno")]
	public string telno { get; set; }
}

public class UpdateUserIdentityResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "full_name")]
		public string full_name { get; set; }

		[JsonPropertyName(name: "telno")]
		public string telno { get; set; }
	}
}

public class ChangePasswordRequestBody {
	[JsonPropertyName(name: "current_pass")]
	public string? current_pass { get; set; }

	[Required]
	[JsonPropertyName(name: "new_pass")]
	public string new_pass { get; set; }

	/// Default will logout from everywhere
	[Required]
	[JsonPropertyName(name: "logout_everywhere")]
	public bool logout_everywhere { get; set; } = true;
}

public class GetUserProfileResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "email")]
		public string email { get; set; }

		[JsonPropertyName(name: "wallet_address")]
		public string wallet_address { get; set; }

		[JsonPropertyName(name: "has_password")]
		public bool has_password { get; set; }
	}
}

public class GetUserBalanceResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "ada_balance")]
		public decimal ada_balance { get; set; }
	}
}

public class RequestLinkExternalWalletResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "signature")]
		public string signature { get; set; }
	}
}

public class LinkExternalWalletRequestBody {
	/// Name of the external wallet. For eg,. Nami, Yoroi, Flint,...
	[Required]
	[JsonPropertyName(name: "wallet_name")]
	public string wallet_name { get; set; }

	/// Raw address in hex
	[Required]
	[JsonPropertyName(name: "wallet_address_in_hex")]
	public string wallet_address_in_hex { get; set; }

	/// Response from external wallet provider
	[Required]
	[JsonPropertyName(name: "signed_signature")]
	public string signed_signature { get; set; }

	/// Response from external wallet provider
	[Required]
	[JsonPropertyName(name: "signed_key")]
	public string signed_key { get; set; }

	[Required]
	[Range(1, 3)]
	[JsonPropertyName(name: "client_type")]
	public UserAuthModelConst.ClientType client_type { get; set; }
}

public class LinkExternalWalletResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
	}
}
