namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

[Table(DbConst.table_sport_user_bet)]
[Index(nameof(user_id), nameof(sport_match_id))]
public class SportUserBetModel : AutoGenerateUpdateTime {
	[Key]
	[Column("id")]
	public long id { get; set; }

	[ForeignKey(DbConst.table_user)]
	[Required]
	[Column("user_id")]
	public Guid user_id { get; set; }

	[ForeignKey(DbConst.table_sport_match)]
	[Required]
	[Column("sport_match_id")]
	public long sport_match_id { get; set; }

	[Required]
	[Column("bet_market_name", TypeName = "varchar(64)"), MaxLength(64)]
	public string bet_market_name { get; set; }

	/// We use string type instead of integer since at some case,
	/// name of odd maybe different.
	[Required]
	[Column("bet_odd_name", TypeName = "varchar(64)"), MaxLength(64)]
	public string bet_odd_name { get; set; }

	/// At this time (2023), each odd has at most 3 decimals and value is in range (1, 100].
	/// But take care of odd spec maybe changed in future, we set precision to 10, scale to 9.
	[Column("bet_odd_value")]
	[Precision(19, 9)]
	public decimal bet_odd_value { get; set; }

	/// Which currency that user will use to pay the order.
	/// For eg,. USD, ADA, ETH, BITCOIN, VND,...
	[Required]
	[ForeignKey(DbConst.table_mst_currency)]
	[Column("bet_currency_id")]
	public int bet_currency_id { get; set; }

	/// The currency amount that user has bet (deposited) to the match, for eg,. 23.150123456789123.
	/// We use max 38 digits value to store `precision (at most 23 digits)` and `scale (at most 15 digits)` in sqlserver.
	/// Note: for blockchain, we just multiply this with decimals to get actual token quantity.
	[Required]
	[Column("bet_currency_amount")]
	[Precision(38, 15)]
	public decimal bet_currency_amount { get; set; }

	[Column("bet_result", TypeName = "tinyint")]
	public SportUserBetModelConst.BetResult bet_result { get; set; }

	/// Which address to be received reward if win.
	[Required]
	[Column("reward_address", TypeName = "varchar(255)"), MaxLength(255)]
	public string reward_address { get; set; }

	/// When user places a bet, we schedule and try (some times) to submit it to blockchain.
	/// This field holds status of that tx.
	[Column("submit_tx_status", TypeName = "tinyint")]
	public SportUserBetModelConst.TxStatus submit_tx_status { get; set; }

	[Column("submit_tx_id", TypeName = "varchar(255)"), MaxLength(255)]
	public string? submit_tx_id { get; set; }

	[Column("submit_tx_result_message", TypeName = "varchar(255)"), MaxLength(255)]
	public string? submit_tx_result_message { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }

	/// FK models (property name must be same with table name)
	public UserModel user { get; set; }
	public SportMatchModel sport_match { get; set; }
	public MstCurrencyModel mst_currency { get; set; }
}

public class SportUserBetModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<SportUserBetModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}

public class SportUserBetModelConst {
	public enum BetResult {
		Nothing = 0,
		Won = 1,
		Draw = 2,
		Losed = 3,
	}

	public enum TxStatus {
		Draft = 1, // Indicates the bet is draft, and should be changed to other status otherwise we don't handle it.
		SubmitSucceed = 3, // The tx was submitted to chain and still in verifying by other nodes
		SubmitFailed = 4, // Could not submit tx to chain (since does not enough balance,...)
		OnchainSucceed = 5, // Finally the tx was published on chain (other nodes got/verified our tx)
	}
}
