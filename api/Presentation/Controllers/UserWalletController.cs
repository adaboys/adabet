namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tool.Compet.Core;

[ApiController, Route(Routes.api_prefix)]
public class UserWalletController : BaseController {
	private readonly UserWalletService service;

	public UserWalletController(UserWalletService service) {
		this.service = service;
	}

	/// <summary>
	/// Request link external wallet to the user.
	/// </summary>
	/// <param name="wallet_address">Address of external wallet (Yoroi, Nami,...). For eg,.
	/// const wallet_address_in_hex = await this.API.getChangeAddress();
	/// const wallet_address = Address.from_bytes(Buffer.from(wallet_address_in_hex, "hex")).to_bech32()
	/// </param>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.user_extwallet_requestLink)]
	public async Task<ActionResult<ApiResponse>> RequestLinkExternalWallet([FromRoute] string wallet_address) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await this.service.RequestLinkExternalWallet(userId.Value, wallet_address.Trim());
	}

	/// <summary>
	/// Perform link external wallet to the user.
	/// </summary>
	/// <param name="wallet_address">Address of external wallet (Yoroi, Nami,...). For eg,.
	/// const wallet_address_in_hex = await this.API.getChangeAddress();
	/// const wallet_address = Address.from_bytes(Buffer.from(wallet_address_in_hex, "hex")).to_bech32()
	/// </param>
	/// <param name="payload">
	/// - wallet_name: Name of the external wallet. For eg,. Nami, Yoroi, Flint,...
	/// - wallet_address_in_hex: Address-in-hex of external wallet (Yoroi, Nami,...). For eg,.
	/// const wallet_address_in_hex = await this.API.getChangeAddress();
	/// - signed_signature: Signature obtained from external wallet after user has logged in.
	/// - signed_key: Key obtained from external wallet after user has logged in.
	/// - client_type: 1 (android-game), 2 (ios-game), 3 (webgl-game), 4 (marketplace-notgame).
	/// </param>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.user_extwallet_link)]
	public async Task<ActionResult<ApiResponse>> LinkExternalWallet([FromRoute] string wallet_address, [FromBody] LinkExternalWalletRequestBody payload) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		DkReflections.TrimJsonAnnotatedProperties(payload);

		return await this.service.LinkExternalWallet(userId.Value, wallet_address, payload);
	}

	/// <summary>
	/// Get all linked external wallets of the user.
	/// </summary>
	/// <response code="200">
	/// - status: 1 (pending), 2 (active), 3 (inactive).
	/// </response>
	[Authorize]
	[HttpGet, Route(Routes.user_extwallet_linked_wallets)]
	public async Task<ActionResult<ApiResponse>> GetLinkedExternalWallets([FromQuery] bool query_balance = true) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await this.service.GetLinkedExternalWallets(userId.Value, query_balance);
	}

	/// <summary>
	/// Get all wallets of the user.
	/// </summary>
	/// <response code="200">
	/// - status: 1 (pending), 2 (active), 3 (inactive).
	/// </response>
	[Authorize]
	[HttpGet, Route(Routes.user_wallets)]
	public async Task<ActionResult<ApiResponse>> GetAllWallets([FromQuery] bool query_balance = true) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await this.service.GetAllWallets(userId.Value, query_balance);
	}

	/// <summary>
	/// Unlink an external wallet from the user.
	/// </summary>
	/// <response code="200">
	/// - code: nfts_mustnot_onsale_or_syncing (All NFTs must NOT be on-sale or syncing)
	/// </response>
	[Authorize]
	[HttpPost, Route(Routes.user_extwallet_unlink)]
	public async Task<ActionResult<ApiResponse>> UnlinkExternalWallet([FromRoute] string wallet_address) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await this.service.UnlinkExternalWallet(userId.Value, wallet_address);
	}

	/// <summary>
	/// Step 1/2 when withdraw to target address.
	/// After call this, client should goto step 2/2 to perform transaction.
	/// </summary>
	/// <response code="200">
	/// - code: invalid_address (the wallet address is not exist),
	/// 				balance_not_enough (wallet balance is not enough).
	/// </response>
	[Authorize]
	[HttpPost, Route(Routes.coin_withdraw_prepare)]
	public async Task<ActionResult<ApiResponse>> PrepareWithdraw([FromBody] WithdrawCoinRequestBody payload) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		DkReflections.TrimJsonAnnotatedProperties(payload);

		return await this.service.PrepareWithdraw(userId.Value, payload);
	}

	/// <summary>
	/// Step 2/2 when withdraw to target address.
	/// After call this, the transaction will be actual sent to Cardano.
	/// </summary>
	/// <response code="200">
	/// - code: invalid_address (the wallet address is not exist),
	/// 				balance_not_enough (wallet balance is not enough).
	/// </response>
	[Authorize]
	[HttpPost, Route(Routes.coin_withdraw_actual)]
	public async Task<ActionResult<ApiResponse>> PerformWithdrawActual([FromBody] PerformWithdrawActualRequestBody payload) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		DkReflections.TrimJsonAnnotatedProperties(payload);

		return await this.service.PerformWithdrawActual(userId.Value, payload);
	}
}
