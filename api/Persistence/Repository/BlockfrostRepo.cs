namespace App;

using System;
using Microsoft.Extensions.Options;
using Tool.Compet.Http;

public class BlockfrostRepo : BaseRepo {
	public BlockfrostRepo(
		IOptionsSnapshot<AppSetting> snapshot
	) : base(snapshot) {
	}

	private DkHttpClient httpClientBlockFrost {
		get {
			// Don't make it static since each task holds different request setting.
			var httpClient = new DkHttpClient();

			// Attach api key to `Authorization` header entry.
			// TechNote: since restriction of CORS, we should not use custom header entry.
			// Lets use safe pre-defined header entries.
			httpClient.SetRequestHeaderEntry("project_id", this.appSetting.blockfrost.apiKey);

			return httpClient;
		}
	}

	public async Task<BlockFrost_GetTxResponse?> GetTransactionAsync(string txHash) {
		var url = $"{this.appSetting.blockfrost.apiBaseUrl}/v0/txs/{txHash}";

		return await this.httpClientBlockFrost.GetForTypeAsync<BlockFrost_GetTxResponse>(url);
	}

	public async Task<BlockFrost_GetTxUtxosResponse?> GetTransactionUtxosAsync(string txHash) {
		var url = $"{this.appSetting.blockfrost.apiBaseUrl}/v0/txs/{txHash}/utxos";

		return await this.httpClientBlockFrost.GetForTypeAsync<BlockFrost_GetTxUtxosResponse>(url);
	}

	public async Task<BlockFrost_GetAssetResponse?> GetAssetAsync(string asset_id_without_dot) {
		var url = $"{this.appSetting.blockfrost.apiBaseUrl}/v0/assets/{asset_id_without_dot}";

		return await this.httpClientBlockFrost.GetForTypeAsync<BlockFrost_GetAssetResponse>(url);
	}

	public async Task<BlockFrost_GetTransactionsResponse[]?> GetTransactionsAsync(string address, int pagePos, int pageSize) {
		var url = $"{this.appSetting.blockfrost.apiBaseUrl}/v0/addresses/{address}/transactions?page={pagePos}&count={pageSize}&order=desc";

		return await this.httpClientBlockFrost.GetForTypeAsync<BlockFrost_GetTransactionsResponse[]>(url);
	}
}
