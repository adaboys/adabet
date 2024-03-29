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
		[JsonPropertyName(name: "name")]
		public string name { get; set; }

		[JsonPropertyName(name: "player_name")]
		public string player_name { get; set; }

		[JsonPropertyName(name: "email")]
		public string email { get; set; }

		[JsonPropertyName(name: "code")]
		public string? code { get; set; }

		[JsonPropertyName(name: "telno")]
		public string? telno { get; set; }

		[JsonPropertyName(name: "referral_code")]
		public string? referral_code { get; set; }

		[JsonPropertyName(name: "wallet_address")]
		public string wallet_address { get; set; }

		[JsonPropertyName(name: "has_password")]
		public bool has_password { get; set; }

		[JsonPropertyName(name: "avatar")]
		public string? avatar { get; set; }

		[JsonPropertyName(name: "vip_level")]
		public int vip_level { get; set; }

		[JsonPropertyName(name: "cur_vip_point")]
		public int cur_vip_point { get; set; }

		[JsonPropertyName(name: "next_vip_point")]
		public int next_vip_point { get; set; }
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

public class UpdateUserAvatarRequestBody {
	[Required]
	[JsonPropertyName(name: "avatar")]
	public IFormFile avatar { get; set; }
}
public class UpdateUserAvatarResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "avatar")]
		public string? avatar { get; set; }
	}
}

public class GetUserListResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "page_pos")]
		public int page_pos { get; set; }

		[JsonPropertyName(name: "page_count")]
		public int page_count { get; set; }

		[JsonPropertyName(name: "total_item_count")]
		public int total_item_count { get; set; }

		[JsonPropertyName(name: "users")]
		public User[] users { get; set; }
	}

	public class User {
		[JsonPropertyName(name: "id")]
		public Guid id { get; set; }

		[JsonPropertyName(name: "avatar")]
		public string? avatar { get; set; }

		[JsonPropertyName(name: "name")]
		public string? name { get; set; }

		[JsonPropertyName(name: "player")]
		public string player { get; set; }

		[JsonPropertyName(name: "wallet")]
		public string wallet_address { get; set; }

		[JsonPropertyName(name: "balance")]
		public List<UserWalletBalance>? wallet_balance { get; set; } = null;

		[JsonPropertyName(name: "role")]
		public int role { get; set; }

		[JsonPropertyName(name: "status")]
		public int status { get; set; }

		[JsonPropertyName(name: "login_locked_until")]
		public DateTime? login_locked_until { get; set; }

		[JsonPropertyName(name: "created_at")]
		public DateTime? created_at { get; set; }

		[JsonPropertyName(name: "last_login")]
		public DateTime? last_login { get; set; }
	}

	public class UserWalletBalance : WalletBalance {
		[JsonPropertyName(name: "coin_name")]
		public string coin_name { get; set; }
	}
}

public class UpdateUserRequestBody {
	[JsonPropertyName(name: "block_login")]
	public bool? block_login { get; set; }

	[JsonPropertyName(name: "block_user")]
	public bool? block_user { get; set; }
}
