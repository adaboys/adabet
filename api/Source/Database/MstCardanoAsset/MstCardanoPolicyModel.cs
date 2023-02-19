namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

/// Policy info at Cardano.
[Table(DbConst.table_mst_cardano_policy)]
[Index(nameof(policy_id), IsUnique = true)]
[Index(nameof(policy_type))]
public class MstCardanoPolicyModel : AutoGenerateUpdateTime {
	/// PK
	[Key]
	[Column("id")]
	public int id { get; set; }

	/// Unique policy id at Cardano blockchain.
	/// In general, it is policy id of token, NFT,...
	[Required]
	[Column("policy_id", TypeName = "varchar(255)"), MaxLength(255)]
	public string policy_id { get; set; }

	/// [See const] Which type of policy (Token, NFT,...)
	[Required]
	[Column("policy_type", TypeName = "tinyint")]
	public MstCardanoPolicyModelConst.PolicyType policy_type { get; set; }

	/// When the policy will be expired.
	/// If it is null, then never-expired.
	[Column("expired_at")]
	public DateTime? expired_at { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }
}
