namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

/// Native token in Cardano.
[Table(DbConst.table_mst_cardano_coin)]
[Index(nameof(coin_name), IsUnique = true)]
[Index(nameof(asset_id), IsUnique = true)]
public class MstCardanoCoinModel : AutoGenerateUpdateTime {
	/// PK
	[Key]
	[Column("id")]
	public int id { get; set; }

	/// Unique coin name (in our system) which be associated with Cardano token. See `MstNativeTokenModelConst.TOKEN_NAME_*`
	/// For eg,. ADA <=> lovelace, XXX <=> Akas23023sadasd02dsadk.123efab
	[Required]
	[Column("coin_name", TypeName = "varchar(32)"), MaxLength(32)]
	public string coin_name { get; set; }

	/// Unique id of Cardano asset (token).
	[Required]
	[Column("asset_id", TypeName = "varchar(255)"), MaxLength(255)]
	public string asset_id { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }
}
