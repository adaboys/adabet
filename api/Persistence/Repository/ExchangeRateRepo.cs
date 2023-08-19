namespace App;

using Microsoft.Extensions.Options;
using Tool.Compet.Http;

public class ExchangeRateRepo : BaseRepo {
	public ExchangeRateRepo(IOptionsSnapshot<AppSetting> snapshot) : base(snapshot) {
	}

	/// Ref: https://docs.coinapi.io/?shell#endpoints
	private DkHttpClient httpClient {
		get {
			// Don't make it static since each task holds different request setting.
			var httpClient = new DkHttpClient();

			// Attach api key to `Authorization` header entry.
			// TechNote: since restriction of CORS, we should not use custom header entry.
			// Lets use safe pre-defined header entries.
			httpClient.SetRequestHeaderEntry("X-CoinAPI-Key", this.appSetting.exchangeRate.apiKey);

			// If above does not work, try with GET request:
			// var url = $"{this.appSetting.currency.apiBaseUrl}/v1/exchangerate/ADA/USD?apikey={this.appSetting.currency.apiKey}";

			return httpClient;
		}
	}

	public async Task<ExchangeRateResponse?> Ada2Usd() {
		var url = $"{this.appSetting.exchangeRate.apiBaseUrl}/v1/exchangerate/ADA/USD";
		return await this.httpClient.GetForTypeAsync<ExchangeRateResponse>(url);
	}
}
