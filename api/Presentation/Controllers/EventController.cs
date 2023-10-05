namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tool.Compet.Core;

[ApiController, Route(Routes.api_prefix)]
public class EventController : BaseController {
	private readonly AppSetting appSetting;
	private readonly EventService service;

	public EventController(
		IOptionsSnapshot<AppSetting> snapshot,
		EventService service
	) {
		this.appSetting = snapshot.Value;
		this.service = service;
	}

	/// <summary>
	/// Register user into new_register event.
	/// </summary>
	/// <param name="event_name">One of: welcome</param>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.event_register)]
	public async Task<ActionResult<ApiResponse>> RegisterAirdropEvent([FromRoute] string event_name) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}

		return await service.RegisterEvent(userId.Value, event_name);
	}

	/// <summary>
	/// Get list of registered users in the new_register event.
	/// </summary>
	/// <param name="page">Page position from 1, for eg,. 1, 2, 3,...</param>
	/// <param name="item">Number of item in each page, for eg,. 10, 20, 30,...</param>
	/// <response code="200">
	/// - gift_delivery_status: 1 (Processing), 2 (Submitted to chain), 3 (Could not submit to chain)
	/// </response>
	[HttpGet, Route(Routes.event_applicants)]
	public async Task<ActionResult<ApiResponse>> GetAirdropEventParticipants([FromQuery] int page, [FromQuery] int item) {
		// Restricts range to avoid spam
		if (page < 1 || item < 1 || item > 100) {
			return new ApiBadRequestResponse("Invalid range");
		}

		return await service.GetEventApplicants(page, item);
	}
}
