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
	/// Get currency list.
	/// </summary>
	/// <response code="200"></response>
	[HttpGet, Route(Routes.currency_list)]
	public async Task<ActionResult<ApiResponse>> GetUserCurrencyList() {
		return await service.GetCurrencyList(userId);
	}
}
