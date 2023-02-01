namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

/// Store tx that sends rewarded coin to winners.
[Table(DbConst.table_sport_winner_bet_reward_tx)]
[Index(nameof(sport_user_bet_id), IsUnique = true)]
public class SportWinnerBetRewardTxModel : AutoGenerateUpdateTime {
	/// PK
	[Key]
	[Column("id")]
	public long id { get; set; }

	[ForeignKey(DbConst.table_sport_user_bet)]
	[Column("sport_user_bet_id")]
	public long sport_user_bet_id { get; set; }

	/// Address for sending the reward.
	[Required]
	[Column("sender_address", TypeName = "varchar(255)"), MaxLength(255)]
	public string sender_address { get; set; }

	/// Address for receiving the reward.
	[Required]
	[Column("receiver_address", TypeName = "varchar(255)"), MaxLength(255)]
	public string receiver_address { get; set; }

	/// Amount of ADA that winner will reward.
	/// In general, it equals to `bet_ada_amount * odd_value`
	[Column("reward_ada_amount")]
	[Precision(19, 6)]
	public decimal reward_ada_amount { get; set; }

	/// Fee (in ADA) of this transaction that the payer has paid to Cardano.
	[Column("fee_in_ada")]
	[Precision(19, 6)]
	public decimal fee_in_ada { get; set; }

	/// [See const] Status of this transaction (pending, done,...).
	[Column("tx_status", TypeName = "tinyint")]
	public SportWinnerBetRewardTxModelConst.TxStatus tx_status { get; set; }

	/// Tx id from Cardano.
	[Column("tx_id", TypeName = "varchar(255)"), MaxLength(255)]
	public string? tx_id { get; set; }

	/// Tx message from Cardano.
	[Column("tx_result_message", TypeName = "varchar(255)"), MaxLength(255)]
	public string? tx_result_message { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }

	/// FK models (property name must be same with table name)
	public SportUserBetModel sport_user_bet { get; set; }
}
