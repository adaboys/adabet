namespace App;

using Microsoft.Extensions.Options;
using Tool.Compet.Http;

public class OnebetRepo : BaseRepo {
	public OnebetRepo(IOptionsSnapshot<AppSetting> snapshot) : base(snapshot) {
	}

	private DkHttpClient httpClient {
		get {
			// Don't make it static since each task holds different request setting.
			var httpClient = new DkHttpClient();

			httpClient.SetAuthorization(this.appSetting.onebet.authToken, "");

			return httpClient;
		}
	}

	public async Task<Onebet_DataMatch[]?> FetchAllLiveMatches() {
		var url = $"{this.appSetting.onebet.apiBaseUrl}/football/live/all";
		return await this.httpClient.GetForType<Onebet_DataMatch[]>(url);
	}
}
