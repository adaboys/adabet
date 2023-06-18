namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tool.Compet.Core;

[ApiController, Route(Routes.api_prefix)]
public class SportController : BaseController {
	private readonly SportService service;

	public SportController(SportService service) {
		this.service = service;
	}

	/// <summary>
	/// Search matches with filter, sorting.
	/// Admin can perform many actions on each match, for eg,. Make it predictable/unpredictable.
	/// </summary>
	/// <param name="match_ids">Target match ids (can specify multiple). For eg,. "92w,34,182"</param>
	/// <param name="sort_type">Format field_asc or field_desc. Where field is: created_at, start_at</param>
	/// <response code="200"></response>
	[RequireAdminDk]
	[HttpGet, Route(Routes.sport_match_list)]
	public async Task<ActionResult<ApiResponse>> SearchMatches(
		[FromRoute] int sport_id,
		[FromQuery] string? team_name,
		[FromQuery] string? match_ids,
		[FromQuery] int page,
		[FromQuery] int item,
		[FromQuery] string? sort_type
	) {
		// Restricts range to avoid spam
		if (page < 1 || item < 1 || item > 100) {
			return new ApiBadRequestResponse("Invalid range");
		}
		return await service.SearchMatches(
			sport_id: sport_id,
			team_name: team_name,
			match_ids: match_ids,
			pagePos: page,
			pageSize: item,
			sortType: sort_type
		);
	}

	/// <summary>
	/// Update a match.
	/// </summary>
	/// <response code="200"></response>
	[RequireAdminDk]
	[HttpPost, Route(Routes.sport_match_update)]
	public async Task<ActionResult<ApiResponse>> UpdateMatch([FromRoute] long match_id, [FromBody] UpdateMatchRequestBody requestBody) {
		return await service.UpdateMatch(match_id, requestBody);
	}
}
