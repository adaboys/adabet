namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tool.Compet.Core;

[ApiController, Route(Routes.api_prefix)]
public class UserWalletController : BaseController {
	private readonly UserWalletService userWalletService;

	public UserWalletController(UserWalletService userWalletService) {
		this.userWalletService = userWalletService;
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
		return await this.userWalletService.RequestLinkExternalWallet(userId.Value, wallet_address.Trim());
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

		return await this.userWalletService.LinkExternalWallet(userId.Value, wallet_address, requestBody);
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
		return await this.userWalletService.GetLinkedExternalWallets(userId.Value);
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
		return await this.userWalletService.UnlinkExternalWallet(userId.Value, wallet_address);
	}
}
