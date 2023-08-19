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
	public async Task<ActionResult<ApiResponse>> PlaceBet([FromRoute] int sport_id, [FromBody] Sport_PlaceBetPayload payload) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}

		// To avoid decode json -> object, when response raw Json, just use Content with type as `application/json`.
		// public async Task<ActionResult> GetSportList() {
		// 	return Content("{\"code\": \"ahihi\"}", "application/json");
		// }

		return await service.PlaceBet(userId.Value, sport_id, payload);
	}

	/// <summary>
	/// [Step 1/2] Request place_bet of given sport via external wallet.
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.sport_extwallet_bet_request)]
	public async Task<ActionResult<ApiResponse>> RequestPlaceBetViaExtWallet([FromRoute] int sport_id, [FromBody] RequestBetViaExtWalletPayload payload) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await service.RequestPlaceBetViaExtWallet(userId.Value, sport_id, payload);
	}

	/// <summary>
	/// [Step 2/2] Report place_bet of given sport via external wallet.
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.sport_extwallet_bet_report)]
	public async Task<ActionResult<ApiResponse>> ReportPlaceBetWithExtWallet([FromRoute] int sport_id, [FromBody] ReportBetViaExtWalletPayload payload) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await service.ReportPlaceBetWithExtWallet(userId.Value, sport_id, payload);
	}

	/// <summary>
	/// Get bet histories of the user.
	/// </summary>
	/// <param name="tab">One of: open, won, lost</param>
	/// <response code="200">
	/// - status: 1 (upcoming), 2 (in-play), 3 (ended). For other status, just consider as `...`
	/// - reward_status: 1 (processing), 2 (submitted), 3 (submit failed).
	/// - bet_result: 0 (unknown), 1 (won), 2 (draw), 3 (losed)
	/// </response>
	[Authorize]
	[HttpGet, Route(Routes.sport_user_bet_histories)]
	public async Task<ActionResult<ApiResponse>> GetBetHistories(
		[FromRoute] int sport_id,
		[FromQuery] int page,
		[FromQuery] int item,
		[FromQuery] string? tab = null
	) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		// Restricts range to avoid spam
		if (page < 1 || item < 1 || item > 100) {
			return new ApiBadRequestResponse("Invalid range");
		}

		return await service.GetBetHistories(userId.Value, sport_id, page, item, tab);
	}

	/// <summary>
	/// Get badges (bet, favorites, ...).
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpGet, Route(Routes.user_sport_badges)]
	public async Task<ActionResult<ApiResponse>> GetBadges([FromRoute] int sport_id) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await service.GetBadges(userId.Value, sport_id);
	}
}
