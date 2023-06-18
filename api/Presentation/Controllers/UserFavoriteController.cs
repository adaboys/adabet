namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tool.Compet.Core;

[ApiController, Route(Routes.api_prefix)]
public class UserFavoriteController : BaseController {
	private readonly UserFavoriteService service;

	public UserFavoriteController(UserFavoriteService service) {
		this.service = service;
	}

	/// <summary>
	/// Favorite a match.
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.user_match_favorite_toggle)]
	public async Task<ActionResult<ApiResponse>> ToggleFavoriteMatch(
		[FromRoute] long match_id,
		[FromQuery] bool toggle_on = true
	) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await this.service.ToggleFavoriteMatch(userId.Value, match_id, toggle_on);
	}

	/// <summary>
	/// Get list of favorite match.
	/// </summary>
	/// <response code="200">
	/// - status: Match status. 1 (Upcoming), 2 (InPlay), ...
	/// </response>
	[Authorize]
	[HttpGet, Route(Routes.user_sport_favorite_list)]
	public async Task<ActionResult<ApiResponse>> GetListOfFavoriteMatch(
		[FromRoute] int sport_id,
		[FromQuery] int page,
		[FromQuery] int item
	) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		// Restricts range to avoid spam
		if (page < 1 || item < 1 || item > 100) {
			return new ApiBadRequestResponse("Invalid range");
		}
		return await this.service.GetListOfFavoriteMatch(userId.Value, sport_id, page, item);
	}
}
