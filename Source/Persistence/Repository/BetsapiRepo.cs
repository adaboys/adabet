namespace App;

using System.Text;
using Microsoft.Extensions.Options;
using Tool.Compet.Http;

public class BetsapiRepo : BaseRepo {
	public BetsapiRepo(IOptionsSnapshot<AppSetting> snapshot) : base(snapshot) {
	}

	private DkHttpClient httpClient {
		get {
			// Don't make it static since each task holds different request setting.
			var httpClient = new DkHttpClient();

			return httpClient;
		}
	}

	private string ApiUrl(int sport_id, string suffix, params string[] query_params) {
		var sb = new StringBuilder(128)
			.Append(this.appSetting.betsapi.apiBaseUrl)
			.Append(suffix)
			.Append("token=").Append(this.appSetting.betsapi.apiKey)
			.Append("sport_id=").Append(sport_id)
		;

		if (query_params != null && query_params.Length > 0) {
			sb.Append('&').Append(string.Join('&', query_params));
		}

		return sb.ToString();
	}

	public async Task<Betsapi_InplayMatchesData?> FetchInplayMatches(int sport_id) {
		var url = ApiUrl(sport_id, "v3/events/inplay");
		return await this.httpClient.GetForType<Betsapi_InplayMatchesData>(url);
	}

	public async Task<Betsapi_OddsSummaryData?> FetchOddsSummary(int sport_id, string betsapi_match_id) {
		var url = ApiUrl(sport_id, "v2/event/odds/summary", $"event_id={betsapi_match_id}");
		return await this.httpClient.GetForType<Betsapi_OddsSummaryData>(url);
	}
}
