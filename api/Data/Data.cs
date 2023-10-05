namespace App;

public class ErrCode {
	public const string invalid_attempt = "invalid_attempt";
	public const string blocked = "blocked";

	public const string must_link_wallet = "must_link_wallet";
	public const string cur_pass_not_match = "cur_pass_not_match";
	public const string otp_expired = "otp_expired";
	public const string invalid_address = CardanoNodeConst.invalid_address;
	public const string balance_not_enough = CardanoNodeConst.balance_not_enough;
	public const string user_existed = "user_existed";
	public const string invalid_register = "invalid_register";
	public const string duplicated_register = "duplicated_register";
	public const string invalid_otp = "invalid_otp";

	public const string email_required = "email_required";
	public const string otp_required = "otp_required";
	public const string invalid_data = "invalid_data";

	public const string market_unavailable = "market_unavailable";
	public const string not_found_match = "not_found_match";
	public const string not_found_market = "not_found_market";
	public const string not_found_odd = "not_found_odd";
	public const string odd_changed = "odd_changed";
	public const string invalid_match = "invalid_match";

	public const string room_full = "room_full";
	public const string already_registered = "already_registered";
}
