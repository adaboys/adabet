namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route(Routes.api_prefix)]
public class UserSportController : BaseController {
	private readonly UserSportService service;

	public UserSportController(UserSportService service) {
		this.service = service;
	}

	/// <summary>
	/// Place bet of given sport.
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.sport_bet_place)]
	public async Task<ActionResult<ApiResponse>> PlaceBet([FromRoute] int sport_id, [FromBody] Sport_PlaceBetRequestBody requestBody) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}

		// To avoid decode json -> object, when response raw Json, just use Content with type as `application/json`.
		// public async Task<ActionResult> GetSportList() {
		// 	return Content("{\"code\": \"ahihi\"}", "application/json");
		// }

		return await service.PlaceBet(userId.Value, sport_id, requestBody);
	}
}
