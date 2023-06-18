namespace App;

using System;

public class MailTemplate {
	public static async Task<string> ForAttemptRegisterUser(string otp_code, int timeout) {
		var mailContent = await File.ReadAllTextAsync("./Assets/MailTemplate/attempt-register-user.html");

		return mailContent
			.Replace("{{otp_code}}", otp_code)
			.Replace("{{timeout}}", $"{timeout}")
		;
	}

	public static async Task<string> ForResetPassword(string otp_code, int timeout) {
		var mailContent = await File.ReadAllTextAsync("./Assets/MailTemplate/reset-password.html");

		return mailContent
			.Replace("{{otp_code}}", otp_code)
			.Replace("{{timeout}}", $"{timeout}");
	}

	public static async Task<string> ForChangePassword() {
		return await File.ReadAllTextAsync("./Assets/MailTemplate/change-password.html");
	}

	public static async Task<string> ForBlockLogin(int locked_hour, string contact_email, string reset_pwd_url) {
		var mailContent = await File.ReadAllTextAsync("./Assets/MailTemplate/notify-tmp-block-login.html");

		return mailContent
			.Replace("{{locked_hour}}", $"{locked_hour}")
			.Replace("{{reset_pwd_url}}", reset_pwd_url)
			.Replace("{{contact_email}}", contact_email);
	}

	public static async Task<string> ForWithdrawCoin_SendOtp(string otp_code, int timeout) {
		var mailContent = await File.ReadAllTextAsync("./Assets/MailTemplate/withdraw-prepare.html");

		return mailContent
			.Replace("{{otp_code}}", otp_code)
			.Replace("{{timeout}}", $"{timeout}")
		;
	}

	public static async Task<string> ForWithdrawCoin_SendActual(
		string sender_address,
		string receiver_address,
		decimal ada_amount,
		decimal abe_amount,
		decimal gem_amount,
		decimal tx_fee
	) {
		var mailContent = await File.ReadAllTextAsync("./Assets/MailTemplate/withdraw-actual.html");

		return mailContent
			.Replace("{{sender_address}}", sender_address)
			.Replace("{{receiver_address}}", receiver_address)
			.Replace("{{ada_amount}}", ada_amount.ToString())
			.Replace("{{abe_amount}}", abe_amount.ToString())
			.Replace("{{gem_amount}}", gem_amount.ToString())
			.Replace("{{tx_fee}}", tx_fee.ToString())
		;
	}
}
