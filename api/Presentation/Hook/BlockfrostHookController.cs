namespace App;

using Microsoft.AspNetCore.Mvc;

[ApiController, Route("blockfrost/hook")]
public class BlockfrostHookController : BaseController {
	private readonly BlockfrostHookService service;

	public BlockfrostHookController(BlockfrostHookService service) {
		this.service = service;
	}

	/// <summary>
	/// Called when new transaction was persisted on Cardano chain.
	/// </summary>
	/// <response code="200"></response>
	[HttpPost, Route("transaction")]
	public async Task<ActionResult<ApiResponse>> OnNewTransaction([FromBody] OnNewTransactionRequestBody requestBody) {
		if (requestBody.type != "transaction") {
			return new ApiBadRequestResponse("Field `type` must be `transaction`");
		}
		return await service.OnNewTransaction(requestBody);
	}
}
