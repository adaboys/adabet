namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route(Routes.api_prefix)]
public class SportController : BaseController {
	private readonly SportService service;

	public SportController(SportService service) {
		this.service = service;
	}

	/// <summary>
	/// Get available sport list.
	/// </summary>
	/// <response code="200"></response>
	[HttpGet, Route(Routes.sports)]
	public async Task<ActionResult<ApiResponse>> GetSportList() {
		return await service.GetSportList();
	}

	/// <summary>
	/// Get all highlight matches.
	/// </summary>
	/// <response code="200">
	/// - img: Name of flag image. Each image has 3 sizes, can get full path with format:
	///   - For small: https://assets.b365api.com/images/team/s/{img_id_here}.png
	///   - For medium: https://assets.b365api.com/images/team/m/{img_id_here}.png
	///   - For large: https://assets.b365api.com/images/team/b/{img_id_here}.png
	/// </response>
	[HttpGet, Route(Routes.sport_matches_highlight)]
	public async Task<ActionResult<ApiResponse>> GetHighlightMatches([FromRoute] int sport_id, [FromQuery] int league) {
		return await service.GetHighlightMatches(sport_id, league);
	}

	/// <summary>
	/// Get all top matches.
	/// </summary>
	/// <response code="200">
	/// - img: Name of flag image. Each image has 3 sizes, can get full path with format:
	///   - For small: https://assets.b365api.com/images/team/s/{img_id_here}.png
	///   - For medium: https://assets.b365api.com/images/team/m/{img_id_here}.png
	///   - For large: https://assets.b365api.com/images/team/b/{img_id_here}.png
	/// </response>
	[HttpGet, Route(Routes.sport_matches_top)]
	public async Task<ActionResult<ApiResponse>> GetTopMatches([FromRoute] int sport_id, [FromQuery] int league) {
		return await service.GetTopMatches(userId, sport_id, league);
	}

	/// <summary>
	/// Get live list of given sport.
	/// </summary>
	/// <response code="200">
	/// - img: Name of flag image. Each image has 3 sizes, can get full path with format:
	///   - For small: https://assets.b365api.com/images/team/s/{img_id_here}.png
	///   - For medium: https://assets.b365api.com/images/team/m/{img_id_here}.png
	///   - For large: https://assets.b365api.com/images/team/b/{img_id_here}.png
	/// </response>
	[HttpGet, Route(Routes.sport_matches_live)]
	public async Task<ActionResult<ApiResponse>> GetLiveMatches([FromRoute] int sport_id, [FromQuery] int league) {
		return await service.GetLiveMatches(userId, sport_id, league);
	}

	/// <summary>
	/// Get upcoming list of given sport.
	/// </summary>
	/// <response code="200">
	/// - img: Name of flag image. Each image has 3 sizes, can get full path with format:
	///   - For small: https://assets.b365api.com/images/team/s/{img_id_here}.png
	///   - For medium: https://assets.b365api.com/images/team/m/{img_id_here}.png
	///   - For large: https://assets.b365api.com/images/team/b/{img_id_here}.png
	/// </response>
	[HttpGet, Route(Routes.sport_matches_upcoming)]
	public async Task<ActionResult<ApiResponse>> GetUpcomingMatches([FromRoute] int sport_id, [FromQuery] int league, [FromQuery] int page, [FromQuery] int item) {
		// Restricts range to avoid spam
		if (page < 1 || item < 1 || item > 100) {
			return new ApiBadRequestResponse("Invalid range");
		}
		return await service.GetUpcomingMatches(userId, sport_id, league, page, item);
	}

	/// <summary>
	/// Get match history of home team and away team.
	/// </summary>
	/// <response code="200">
	/// - status: Match status. One of: 1 (upcoming), 2 (inplay), 3 (ended).
	/// </response>
	[HttpGet, Route(Routes.sport_match_history)]
	public async Task<ActionResult<ApiResponse>> GetMatchHistory([FromRoute] long match_id) {
		return await service.GetMatchHistory(match_id);
	}
}
