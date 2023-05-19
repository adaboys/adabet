namespace App;

using System;
using System.Text;
using Microsoft.Extensions.Options;
using Tool.Compet.Http;

public class CasinoRepo : BaseRepo {
	public CasinoRepo(IOptionsSnapshot<AppSetting> snapshot) : base(snapshot) {
	}

	private DkHttpClient httpClient {
		get {
			var httpClient = new DkHttpClient();

			httpClient.SetAuthorization("Bearer", this.appSetting.casino.apiKey);

			return httpClient;
		}
	}

	public async Task<Casino_RegisterUserResponse?> RegisterUser(Guid user_id, string? name, string email, string? password, string wallet_address) {
		var url = $"{this.appSetting.casino.apiBaseUrl}/s1/user/register";

		return await this.httpClient.PostForType<Casino_RegisterUserResponse>(url, new Casino_RegisterUserRequestBody {
			user_id = user_id,
			name = name,
			email = email,
			password = password,
			wallet = wallet_address
		});
	}
}
