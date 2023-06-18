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
	[HttpPost, Route(Routes.user_external_wallet_requestLink)]
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
	/// <param name="requestBody">
	/// - wallet_name: Name of the external wallet. For eg,. Nami, Yoroi, Flint,...
	/// - wallet_address_in_hex: Address-in-hex of external wallet (Yoroi, Nami,...). For eg,.
	/// const wallet_address_in_hex = await this.API.getChangeAddress();
	/// - signed_signature: Signature obtained from external wallet after user has logged in.
	/// - signed_key: Key obtained from external wallet after user has logged in.
	/// - client_type: 1 (android-game), 2 (ios-game), 3 (webgl-game), 4 (marketplace-notgame).
	/// </param>
	/// <response code="200"></response>
	[Authorize]
	[HttpPost, Route(Routes.user_external_wallet_link)]
	public async Task<ActionResult<ApiResponse>> LinkExternalWallet([FromRoute] string wallet_address, [FromBody] LinkExternalWalletRequestBody requestBody) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await this.service.LinkExternalWallet(userId.Value, wallet_address, requestBody);
	}

	/// <summary>
	/// Get all linked external wallets of the user.
	/// </summary>
	/// <response code="200">
	/// - status: 1 (pending), 2 (active), 3 (inactive).
	/// </response>
	[Authorize]
	[HttpGet, Route(Routes.user_external_wallet_linked_wallets)]
	public async Task<ActionResult<ApiResponse>> GetLinkedExternalWallets() {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		return await this.service.GetLinkedExternalWallets(userId.Value);
	}

	/// <summary>
	/// Unlink an external wallet from the user.
	/// </summary>
	/// <response code="200">
	/// - code: nfts_mustnot_onsale_or_syncing (All NFTs must NOT be on-sale or syncing)
	/// </response>
	[Authorize]
	[HttpPost, Route(Routes.user_external_wallet_unlink)]
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
	public async Task<ActionResult<ApiResponse>> PrepareWithdraw([FromBody] WithdrawCoinRequestBody requestBody) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}
		DkReflections.TrimJsonAnnotatedProperties(requestBody);

		return await this.service.PrepareWithdraw(userId.Value, requestBody);
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
	public async Task<ActionResult<ApiResponse>> PerformWithdrawActual([FromBody] PerformWithdrawActualRequestBody requestBody) {
		if (userId is null) {
			return new ApiForbiddenResponse();
		}

		return await this.service.PerformWithdrawActual(userId.Value, requestBody);
	}
}
