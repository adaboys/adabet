namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route(Routes.api_prefix)]
public class CurrencyController : BaseController {
	private readonly CurrencyService service;

	public CurrencyController(CurrencyService service) {
		this.service = service;
	}

	/// <summary>
	/// Get currency list that user can use to bet with.
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpGet, Route(Routes.user_currency_list)]
	public async Task<ActionResult<ApiResponse>> GetUserCurrencyList() {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await service.GetUserCurrencyList(userId.Value);
	}
}
