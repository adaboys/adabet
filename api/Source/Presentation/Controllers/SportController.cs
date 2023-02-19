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
	/// Get quick access links for leagues.
	/// </summary>
	/// <response code="200"></response>
	[HttpGet, Route(Routes.sport_league_quick_links)]
	public async Task<ActionResult<ApiResponse>> GetQuickLinkOfLeagues() {
		return await service.GetQuickLinkOfLeagues();
	}

	/// <summary>
	/// Get all highlight matches.
	/// </summary>
	/// <response code="200"></response>
	[HttpGet, Route(Routes.sport_matches_highlight)]
	public async Task<ActionResult<ApiResponse>> GetHighlightMatches([FromRoute] int sport_id) {
		return await service.GetHighlightMatches(sport_id);
	}

	/// <summary>
	/// Get all top matches.
	/// </summary>
	/// <response code="200"></response>
	[HttpGet, Route(Routes.sport_matches_top)]
	public async Task<ActionResult<ApiResponse>> GetTopMatches([FromRoute] int sport_id) {
		return await service.GetTopMatches(sport_id);
	}

	/// <summary>
	/// Get live list of given sport.
	/// </summary>
	/// <response code="200"></response>
	[HttpGet, Route(Routes.sport_matches_live)]
	public async Task<ActionResult<ApiResponse>> GetLiveMatches([FromRoute] int sport_id) {
		return await service.GetLiveMatches(sport_id);
	}

	/// <summary>
	/// Get upcoming list of given sport.
	/// </summary>
	/// <response code="200"></response>
	[HttpGet, Route(Routes.sport_matches_upcoming)]
	public async Task<ActionResult<ApiResponse>> GetUpcomingMatches([FromRoute] int sport_id, [FromQuery] int page, [FromQuery] int item) {
		// Restricts range to avoid spam
		if (page < 1 || item < 1 || item > 100) {
			return new ApiBadRequestResponse("Invalid range");
		}
		return await service.GetUpcomingMatches(sport_id, page, item);
	}
}
