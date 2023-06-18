namespace App;

using System;
using System.Text;
using Microsoft.Extensions.Options;
using Tool.Compet.Http;

/// Api doc: https://betsapi.com/docs/events/

/// `tt` is playing or on break. it would be =0 when on half break.
/// `ta` is injury time.
/// `md` is period but that's not reliable sometimes.
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

	private string SportApiUrl(int sport_id, string suffix, params string[] query_params) {
		var sb = new StringBuilder(128)
			.Append(this.appSetting.betsapi.apiBaseUrl)
			.Append(suffix)
			.Append("?token=").Append(this.appSetting.betsapi.apiKey)
			.Append("&sport_id=").Append(sport_id)
		;

		if (query_params != null && query_params.Length > 0) {
			sb.Append('&').Append(string.Join('&', query_params));
		}

		return sb.ToString();
	}

	private string MatchApiUrl(string suffix, params string[] query_params) {
		var sb = new StringBuilder(128)
			.Append(this.appSetting.betsapi.apiBaseUrl)
			.Append(suffix)
			.Append("?token=").Append(this.appSetting.betsapi.apiKey)
		;

		if (query_params != null && query_params.Length > 0) {
			sb.Append('&').Append(string.Join('&', query_params));
		}

		return sb.ToString();
	}

	/// Load all live matches.
	/// Ref: https://betsapi.com/docs/events/inplay.html
	public async Task<Betsapi_InplayMatchesData?> FetchInplayMatches(int sport_id) {
		var url = SportApiUrl(sport_id, "/v3/events/inplay");
		return await this.httpClient.GetForType<Betsapi_InplayMatchesData>(url);
	}

	/// <param name="day">YYYYMMDD format, for eg,. 20230912</param>
	/// <param name="page">Max 100</param>
	public async Task<Betsapi_UpcomingMatchesData?> FetchUpcomingMatches(int sport_id, string day, int page) {
		if (page < 1 || page > 100) {
			throw new AppInvalidArgumentException("Invalid page range ! Must in [1, 100]");
		}
		var url = SportApiUrl(sport_id, "/v3/events/upcoming", $"day={day}&page={page}");
		return await this.httpClient.GetForType<Betsapi_UpcomingMatchesData>(url);
	}

	/// Ref: https://betsapi.com/docs/events/odds_summary.html
	public async Task<Betsapi_OddsSummaryData?> FetchMatchOddsSummary(long match_id) {
		var url = MatchApiUrl("/v2/event/odds/summary", $"event_id={match_id}");
		return await this.httpClient.GetForType<Betsapi_OddsSummaryData>(url);
	}

	/// Limitation: query at most 10 matches each time.
	public async Task<BetsapiData_MatchDetail?> FetchMatchDetail(params long[] match_ids) {
		var url = MatchApiUrl("/v1/event/view", $"event_id={string.Join(',', match_ids)}");
		return await this.httpClient.GetForType<BetsapiData_MatchDetail>(url);
	}

	public async Task<BetsapiData_MatchDetail_Upcoming?> FetchMatchDetail_Upcoming(params long[] match_ids) {
		var url = MatchApiUrl("/v1/event/view", $"event_id={string.Join(',', match_ids)}");
		return await this.httpClient.GetForType<BetsapiData_MatchDetail_Upcoming>(url);
	}

	/// <param name="page">Max 100</param>
	public async Task<BetsapiData_MergeHistory?> FetchMatchMergeHistory(long match_id, long since_time, int page) {
		if (page < 1 || page > 100) {
			throw new AppInvalidArgumentException("Invalid page range ! Must in [1, 100]");
		}
		var url = MatchApiUrl("/v1/event/merge_history", $"since_time={since_time}&page={page}");
		return await this.httpClient.GetForType<BetsapiData_MergeHistory>(url);
	}

	/// <param name="size">One of: s (small), m (medium), b (big)</param>
	public async Task<byte[]?> FetchImage(string image_id, string size = "s") {
		return await this.httpClient.GetForByteArray($"https://assets.b365api.com/images/team/{size}/{image_id}.png");
	}

	/// Some match does not image but the api still response empty image.
	/// If the image size < 1.2 KB, then we consider it is empty.
	public async Task<string?> CalcImageId(string? image_id) {
		if (image_id is null) {
			return null;
		}

		var imageBytes = await this.FetchImage(image_id);
		if (imageBytes == null || imageBytes.Length < 1200) {
			return null;
		}

		return image_id;
	}
}
