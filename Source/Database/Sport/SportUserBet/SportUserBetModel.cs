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

	[Precision(6, 2)]
	[Column("bet_odd_value")]
	public decimal bet_odd_value { get; set; }

	/// ADA amount that user has bet (deposited) to the match.
	[Precision(9, 6)]
	[Column("bet_ada_amount")]
	public decimal bet_ada_amount { get; set; }

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
}
