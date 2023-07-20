namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tool.Compet.Core;

[ApiController, Route(Routes.api_prefix)]
public class UserCoinTxController : BaseController {
	private readonly UserCoinTxService service;

	public UserCoinTxController(UserCoinTxService service) {
		this.service = service;
	}

	/// <summary>
	/// Get coin tx history of the user.
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpGet, Route(Routes.user_coin_tx_history)]
	public async Task<ActionResult<ApiResponse>> GetCoinTxHistory(
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
		return await this.service.GetCoinTxHistory(userId.Value, page, item);
	}
}
