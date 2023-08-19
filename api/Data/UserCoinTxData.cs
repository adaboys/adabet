namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class GetCoinTxHistoryResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "page_pos")]
		public int page_pos { get; set; }

		[JsonPropertyName(name: "page_count")]
		public int page_count { get; set; }

		[JsonPropertyName(name: "total_item_count")]
		public int total_item_count { get; set; }

		[JsonPropertyName(name: "histories")]
		public History[] histories { get; set; }
	}

	public class History {
		[JsonPropertyName(name: "created_at")]
		public DateTime created_at { get; set; }

		[JsonPropertyName(name: "amount")]
		public decimal amount { get; set; }

		[JsonPropertyName(name: "status")]
		public int status { get; set; }

		[JsonPropertyName(name: "tx_hash")]
		public string? tx_hash { get; set; }

		[JsonPropertyName(name: "receiver_address")]
		public string receiver_address { get; set; }

		[JsonPropertyName(name: "sender_address")]
		public string sender_address { get; set; }

		[JsonPropertyName(name: "send_amount")]
		public decimal send_amount { get; set; }

		[JsonPropertyName(name: "receive_amount")]
		public decimal receive_amount { get; set; }

		[JsonPropertyName(name: "attach_ada_amount")]
		public decimal attach_ada_amount { get; set; }

		[JsonPropertyName(name: "send_coin")]
		public int send_coin { get; set; }

		[JsonPropertyName(name: "receive_coin")]
		public int receive_coin { get; set; }

		[JsonPropertyName(name: "fee_in_ada")]
		public decimal fee_in_ada { get; set; }

		[JsonPropertyName(name: "updated_at")]
		public DateTime? updated_at { get; set; }
	}
}

public class SwapCoinPayload {
	[Required]
	[JsonPropertyName(name: "src_coin")]
	public int src_coin { get; set; }

	[Required]
	[JsonPropertyName(name: "dst_coin")]
	public int dst_coin { get; set; }

	[Required]
	[JsonPropertyName(name: "amount")]
	public decimal amount { get; set; }

	[JsonPropertyName(name: "receiver_wallet")]
	public string? receiver_wallet { get; set; }
}

public class GetAllCoinTxHistoryResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "page_pos")]
		public int page_pos { get; set; }

		[JsonPropertyName(name: "has_next_page")]
		public bool hasNextPage { get; set; }

		[JsonPropertyName(name: "total_item_count")]
		public int total_item_count { get; set; }

		[JsonPropertyName(name: "histories")]
		public List<History> histories { get; set; }
	}

	public class History {
		[JsonPropertyName(name: "tx_hash")]
		public string tx_hash { get; set; }

		[JsonPropertyName(name: "receive_amount")]
		public decimal receive_amount { get; set; }

		[JsonPropertyName(name: "created_at")]
		public DateTime created_at { get; set; }
	}
}

public class CalcSwapCoinAmountResponse : ApiSuccessResponse {
	[JsonPropertyName(name: "data")]
	public Data data { get; set; }

	public class Data {
		[JsonPropertyName(name: "amount")]
		public decimal amount { get; set; }
	}
}
