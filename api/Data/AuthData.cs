namespace App;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class VerifyAuthRequestBody {
	[Required]
	[JsonPropertyName(name: "access_token")]
	public string access_token { get; set; }
}

public class VerifyAuthResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public VerifyAuthResponseData data { get; set; }

	public class VerifyAuthResponseData {
		[JsonPropertyName(name: "user_id")]
		public string userId { get; set; }
	}
}

public class LoginRequestBody {
	[Required]
	[JsonPropertyName(name: "email")]
	public string email { get; set; }

	[Required]
	[JsonPropertyName(name: "password")]
	public string password { get; set; }

	[Required]
	[Range(1, 3)]
	[JsonPropertyName(name: "client_type")]
	public byte client_type { get; set; }
}

public class LoginResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "token_schema")]
		public string token_schema { get; set; }

		[JsonPropertyName(name: "access_token")]
		public string access_token { get; set; }

		[JsonPropertyName(name: "refresh_token")]
		public string refresh_token { get; set; }
	}
}

public class ProviderLoginRequestBody {
	[Required]
	[JsonPropertyName(name: "provider")]
	public string provider { get; set; }

	[JsonPropertyName(name: "id_token")]
	public string? id_token { get; set; }

	[JsonPropertyName(name: "access_token")]
	public string? access_token { get; set; }

	[Required]
	[Range(1, 3)]
	[JsonPropertyName(name: "client_type")]
	public UserAuthModelConst.ClientType client_type { get; set; }
}

// Full response:
// {
//   "sub": "110298812918762767322",
//   "name": "Your full name",
//   "given_name": "First name",
//   "family_name": "Midle + Last name",
//   "picture": "https://lh3.googleusercontent.com/a-/XXX-mA\u003ds96-c",
//   "email": "test@gmail.com",
//   "email_verified": true,
//   "locale": "en"
// }
public class GoogleAccessTokenDecodingResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "name")]
	public string name { get; set; }

	[JsonPropertyName(name: "email")]
	public string email { get; set; }
}

public class FacebookAccessTokenDecodingResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "name")]
	public string name { get; set; }

	[JsonPropertyName(name: "email")]
	public string email { get; set; }
}

public class SilentLoginRequestBody {
	[Required]
	[JsonPropertyName(name: "access_token")]
	public string access_token { get; set; }

	[Required]
	[JsonPropertyName(name: "refresh_token")]
	public string refresh_token { get; set; }
}

public class LoginWithTokenRequestBody {
	[Required]
	[JsonPropertyName(name: "access_token")]
	public string access_token { get; set; }

	[Required]
	[JsonPropertyName(name: "refresh_token")]
	public string refresh_token { get; set; }

	[Required]
	[Range(1, 3)]
	[JsonPropertyName(name: "client_type")]
	public byte client_type { get; set; }
}

public class RequestLoginWithExternalWalletRequestBody {
	[Required]
	[JsonPropertyName(name: "wallet_address")]
	public string wallet_address { get; set; }
}

public class RequestLoginWithExternalWalletResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "signature")]
		public string signature { get; set; }
	}
}

public class LoginWithExternalWalletRequestBody {
	/// Raw address in hex
	[Required]
	[JsonPropertyName(name: "wallet_address_in_hex")]
	public string wallet_address_in_hex { get; set; }

	/// Address after converted from raw address via `bench_32()`
	[Required]
	[JsonPropertyName(name: "wallet_address")]
	public string wallet_address { get; set; }

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

	/// For create new user
	[EmailAddress]
	[JsonPropertyName(name: "email")]
	public string? email { get; set; }

	[JsonPropertyName(name: "request_otp")]
	public bool request_otp { get; set; } = false;

	[JsonPropertyName(name: "otp")]
	public string? otp { get; set; }

	[JsonPropertyName(name: "wallet_name")]
	public string? wallet_name { get; set; }
}

public class LogoutRequestBody {
	[Required]
	[JsonPropertyName(name: "refresh_token")]
	public string refresh_token { get; set; }

	[JsonPropertyName(name: "logout_everywhere")]
	public bool logout_everywhere { get; set; } = false;
}

public class RequestResetPasswordRequestBody {
	[Required]
	[JsonPropertyName(name: "email")]
	public string email { get; set; }
}

public class ConfirmResetPasswordRequestBody {
	[Required]
	[JsonPropertyName(name: "otp_code")]
	public string otp_code { get; set; }

	[Required]
	[JsonPropertyName(name: "email")]
	public string email { get; set; }

	[Required]
	[JsonPropertyName(name: "new_pass")]
	public string new_pass { get; set; }

	/// Default will logout from everywhere
	[JsonPropertyName(name: "logout_everywhere")]
	public bool logout_everywhere { get; set; } = true;
}

public class GetUserAuthInfoResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "user_id")]
		public Guid user_id { get; set; }
	}
}
