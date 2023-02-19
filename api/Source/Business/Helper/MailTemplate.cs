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
}
