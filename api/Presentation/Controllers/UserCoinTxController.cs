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
	/// <param name="action">One of: 1 (withdraw), 2 (swap)</param>
	/// <response code="200">
	/// - status: One of: 2 (submit to chain succeed), 3 (submit to chain failed).
	/// </response>
	[Authorize]
	[HttpGet, Route(Routes.coin_tx_history)]
	public async Task<ActionResult<ApiResponse>> GetCoinTxHistory(
		[FromQuery] int action,
		[FromQuery] int? coin_id,
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
		return await this.service.GetCoinTxHistory(userId.Value, (CoinTxModelConst.ActionType)action, coin_id, page, item);
	}

	/// <summary>
	/// Get coin all tx history of the user.
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpGet, Route(Routes.all_tx_history)]
	public async Task<ActionResult<ApiResponse>> GetAllTxHistories(
		[FromQuery] int? coin_id,
		[FromQuery] int page,
		[FromQuery] int item
	) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		// Restricts range to avoid spam
		if (page < 1 || item < 1 || item > 10) {
			return new ApiBadRequestResponse("Invalid range");
		}
		return await this.service.GetAllTxHistories(userId.Value, coin_id, page, item);
	}

	/// <summary>
	/// From src_coin, calculate dst_coin that user can receive after swap.
	/// </summary>
	/// <response code="200"></response>
	[HttpGet, Route(Routes.coin_swap_calc_amount)]
	public async Task<ActionResult<ApiResponse>> CalcSwapCoinAmount([FromQuery] int src_coin, [FromQuery] int dst_coin, [FromQuery] string amount) {
		return await this.service.CalcSwapCoinAmount(src_coin, dst_coin, amount.ParseDecimalDk());
	}

	/// <summary>
	/// Swap coin to given wallet.
	/// </summary>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.coin_swap)]
	public async Task<ActionResult<ApiResponse>> SwapCoin([FromBody] SwapCoinPayload payload) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await this.service.SwapCoin(userId.Value, payload);
	}
}
