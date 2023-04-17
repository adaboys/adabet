namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route(Routes.api_prefix)]
public class SportPredictionController : BaseController {
	private readonly SportPredictionService service;

	public SportPredictionController(SportPredictionService service) {
		this.service = service;
	}

	/// <summary>
	/// Get list of sport prediction match.
	/// </summary>
	/// <response code="200"></response>
	[HttpGet, Route(Routes.sport_prediction_match_list)]
	public async Task<ActionResult<ApiResponse>> GetPredictionMatchList([FromRoute] int sport_id, [FromQuery] int page, [FromQuery] int item) {
		// Restricts range to avoid spam
		if (page < 1 || item < 1 || item > 100) {
			return new ApiBadRequestResponse("Invalid range");
		}
		return await service.GetPredictionMatchList(userId, sport_id, page, item);
	}

	/// <summary>
	/// Get list of sport prediction match.
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.sport_prediction_match_predict)]
	public async Task<ActionResult<ApiResponse>> PredictMatch([FromRoute] long match_id, Sport_PredictMatchRequestBody requestBody) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}

		return await service.PredictMatch(userId.Value, match_id, requestBody);
	}

	/// <summary>
	/// Get predicted user list on the sport match.
	/// </summary>
	/// <response code="200">
	/// - reward_delivery_status: 0 (Nothing), 1 (Submitted to chain), 2 (Could not submit to chain), 3 (Final succeed, other nodes confirmed)
	/// </response>
	[HttpGet, Route(Routes.sport_prediction_match_predicted_users)]
	public async Task<ActionResult<ApiResponse>> GetPredictedUsersForMatch([FromRoute] long match_id, [FromQuery] int page, [FromQuery] int item) {
		return await service.GetPredictedUsersOnMatch(match_id, page, item);
	}

	/// <summary>
	/// Get prediction leaderboard.
	/// </summary>
	/// <param name="coin_id">The coin id, for eg,. 1 (ADA), 2 (ABE), 3 (GEM), ...</param>
	/// <param name="up_to_now">Elapsed time to now. One of: week, month, total</param>
	/// <response code="200"></response>
	[HttpGet, Route(Routes.sport_prediction_leaderboard)]
	public async Task<ActionResult<ApiResponse>> GetLeaderboard([FromQuery] int coin_id, [FromQuery] string up_to_now, [FromQuery] int page, [FromQuery] int item) {
		var upToNowSeconds = up_to_now switch {
			"week" => 7 * 24 * 3600L,
			"month" => 30 * 24 * 3600L,
			"total" => 0,
			_ => -1,
		};
		if (upToNowSeconds < 0) {
			return new ApiBadRequestResponse("Invalid duration");
		}
		return await service.GetLeaderboard(coin_id, upToNowSeconds, page, item);
	}
}
