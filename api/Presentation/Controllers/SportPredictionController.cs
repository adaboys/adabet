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
	public async Task<ActionResult<ApiResponse>> GetSportPredictionMatches([FromRoute] int sport_id, [FromQuery] int page, [FromQuery] int item) {
		return await service.GetSportPredictionMatches(sport_id, page, item);
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
	public async Task<ActionResult<ApiResponse>> GetSportMatchPredictedUsers([FromRoute] long match_id, [FromQuery] int page, [FromQuery] int item) {
		return await service.GetSportMatchPredictedUsers(match_id, page, item);
	}
}
