namespace App;

using Microsoft.AspNetCore.Mvc;

/// This api is at api server (not at api gateway).
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
	public async Task<ActionResult<object>> OnNewTransaction([FromBody] OnNewTransactionRequestBody requestBody) {
		if (requestBody.type != "transaction") {
			return BadRequest("Field `type` must be `transaction`");
		}
		return await service.OnNewTransaction(requestBody);
	}
}
