namespace App;

public class OnNewTransactionRequestBody {
	public string id { get; set; }
	public string webhook_id { get; set; }
	public int created { get; set; }
	public int api_version { get; set; }
	public string type { get; set; }
	public Payload[] payload { get; set; }

	public class Amount {
		public string unit { get; set; }
		public string quantity { get; set; }
	}

	public class Input {
		public string address { get; set; }
		public List<Amount> amount { get; set; }
		public string tx_hash { get; set; }
		public int output_index { get; set; }
		public bool collateral { get; set; }
		public string? data_hash { get; set; }
	}

	public class Output {
		public string address { get; set; }
		public List<Amount> amount { get; set; }
		public int output_index { get; set; }
		public string? data_hash { get; set; }
	}

	public class OutputAmount {
		public string unit { get; set; }
		public string quantity { get; set; }
	}

	public class Payload {
		public Tx tx { get; set; }
		public List<Input> inputs { get; set; }
		public List<Output> outputs { get; set; }
	}

	public class Tx {
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
		public string? invalid_before { get; set; }
		public string? invalid_hereafter { get; set; }
		public int utxo_count { get; set; }
		public int withdrawal_count { get; set; }
		public int mir_cert_count { get; set; }
		public int delegation_count { get; set; }
		public int stake_cert_count { get; set; }
		public int pool_update_count { get; set; }
		public int pool_retire_count { get; set; }
		public int asset_mint_or_burn_count { get; set; }
		public int redeemer_count { get; set; }
		public bool valid_contract { get; set; }
	}
}

public class OnBalanceChangedData {
	public int coin_id { get; set; }
	public decimal amount { get; set; }
}
