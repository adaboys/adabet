namespace App;

using System.Text.Json.Serialization;
using Tool.Compet.Http;

public class BlockFrost_GetTxResponse {
	public bool valid_contract { get; set; }
	public string hash { get; set; }
	public string block { get; set; }
	public int block_height { get; set; }
	public int block_time { get; set; }
	public int slot { get; set; }
	public int index { get; set; }
	public List<OutputAmount> output_amount { get; set; }
	public string fees { get; set; }
	public string deposit { get; set; }
	public int size { get; set; }
	public object invalid_before { get; set; }
	public object invalid_hereafter { get; set; }
	public int utxo_count { get; set; }
	public int withdrawal_count { get; set; }
	public int mir_cert_count { get; set; }
	public int delegation_count { get; set; }
	public int stake_cert_count { get; set; }
	public int pool_update_count { get; set; }
	public int pool_retire_count { get; set; }
	public int asset_mint_or_burn_count { get; set; }
	public int redeemer_count { get; set; }

	public class OutputAmount {
		public string unit { get; set; }
		public string quantity { get; set; }
	}
}

public class BlockFrost_GetAssetResponse {
	public string? error { get; set; }

	public string asset { get; set; }
	public string policy_id { get; set; }
	public string asset_name { get; set; }
	public string fingerprint { get; set; }
	public string quantity { get; set; }
	public string initial_mint_tx_hash { get; set; }
	public int mint_or_burn_count { get; set; }
	public object onchain_metadata { get; set; }
	public object onchain_metadata_standard { get; set; }
	public Metadata? metadata { get; set; }

	/// Unility method for checking successful result.
	[JsonIgnore]
	public bool succeed => this.error == null;

	/// Unility method for checking failure result.
	[JsonIgnore]
	public bool failed => this.error != null;

	public class Metadata {
		public string name { get; set; }
		public string description { get; set; }
		public string ticker { get; set; }
		public string url { get; set; }
		public string logo { get; set; }
		public int decimals { get; set; }
	}
}

public class BlockFrost_GetTxUtxosResponse {
	public string hash { get; set; }
	public List<Input> inputs { get; set; }
	public List<Output> outputs { get; set; }

	public class Amount {
		public string unit { get; set; }
		public string quantity { get; set; }
	}

	public class Input {
		public string address { get; set; }
		public List<Amount> amount { get; set; }
		public string tx_hash { get; set; }
		public int output_index { get; set; }
		public object data_hash { get; set; }
		public object inline_datum { get; set; }
		public object reference_script_hash { get; set; }
		public bool collateral { get; set; }
		public bool reference { get; set; }
	}

	public class Output {
		public string address { get; set; }
		public List<Amount> amount { get; set; }
		public int output_index { get; set; }
		public object data_hash { get; set; }
		public object inline_datum { get; set; }
		public bool collateral { get; set; }
		public object reference_script_hash { get; set; }
	}
}

public class BlockFrost_GetTransactionsResponse {
	public string tx_hash { get; set; }
	public int tx_index { get; set; }
	public int block_height { get; set; }
	public int block_time { get; set; }
}

