namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

/// User: bet with ADA -> reward as ABE
[Table(DbConst.table_sport_predict_user)]
[Index(nameof(user_id), nameof(sport_match_id), IsUnique = true)]
[Index(nameof(predicted_at))]
public class SportPredictUserModel : AutoGenerateUpdateTime {
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
	[Column("predict_home_score")]
	public int predict_home_score { get; set; }

	[Required]
	[Column("predict_away_score")]
	public int predict_away_score { get; set; }

	/// Which address to be received reward if win.
	[Required]
	[Column("reward_address", TypeName = "varchar(255)"), MaxLength(255)]
	public string reward_address { get; set; }

	/// When register new prediction, user must provide reward coin-id to receive if win.
	[Required]
	[ForeignKey(DbConst.table_mst_currency)]
	[Column("reward_coin_id")]
	public int reward_coin_id { get; set; }

	/// It is calculated before send to winners.
	/// We use max 38 digits value to store `precision (at most 23 digits)` and `scale (at most 15 digits)` in sqlserver.
	[Column("reward_coin_amount")]
	[Precision(38, 15)]
	public decimal reward_coin_amount { get; set; }

	/// When user predicts, we schedule and try (some times) to submit it to blockchain.
	/// This field holds status of that tx.
	[Column("bet_submit_tx_status", TypeName = "tinyint")]
	public SportPredictUserModelConst.BetSubmitTxStatus bet_submit_tx_status { get; set; }

	[Column("bet_submit_tx_id", TypeName = "varchar(255)"), MaxLength(255)]
	public string? bet_submit_tx_id { get; set; }

	[Column("bet_submit_tx_result_message", TypeName = "varchar(255)"), MaxLength(255)]
	public string? bet_submit_tx_result_message { get; set; }

	/// When we submit the bet successful, we set prediction time to this.
	[Column("predicted_at")]
	public DateTime? predicted_at { get; set; }

	/// When match ended, we calculate prediction-rank for top winners, and set it here.
	[Column("prediction_rank")]
	public int prediction_rank { get; set; }

	/// When user won the prediction, we schedule and try (some times) to submit it to blockchain.
	/// This field holds status of that tx.
	[Column("reward_submit_tx_status", TypeName = "tinyint")]
	public SportPredictUserModelConst.RewardSubmitTxStatus reward_submit_tx_status { get; set; }

	[Column("reward_submit_tx_id", TypeName = "varchar(255)"), MaxLength(255)]
	public string? reward_submit_tx_id { get; set; }

	[Column("reward_submit_tx_result_message", TypeName = "varchar(255)"), MaxLength(255)]
	public string? reward_submit_tx_result_message { get; set; }

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

public class SportPredictUserModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<SportPredictUserModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}

public class SportPredictUserModelConst {
	public enum BetSubmitTxStatus {
		Nothing = 0, // Null
		SubmitSucceed = 1, // The tx was submitted to chain and still in verifying by other nodes
		SubmitFailed = 2, // Could not submit tx to chain (since does not enough balance,...)
		OnchainSucceed = 3, // Finally the tx was published on chain (other nodes got/verified our tx)
	}

	public enum RewardSubmitTxStatus {
		Nothing = 0, // Null
		RequestSubmitReward = 1, // Need submit the reward to winner
		SubmitSucceed = 2, // The tx was submitted to chain and still in verifying by other nodes
		SubmitFailed = 3, // Could not submit tx to chain (since does not enough balance,...)
		OnchainSucceed = 4, // Finally the tx was published on chain (other nodes got/verified our tx)
	}

	public static readonly BetSubmitTxStatus[] SuccessBetSubmitStatusList = new BetSubmitTxStatus[] {
		BetSubmitTxStatus.SubmitSucceed,
		BetSubmitTxStatus.OnchainSucceed
	};
}
