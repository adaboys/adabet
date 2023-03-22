namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

/// Holds all system wallets for gaming, staking, transactions,...
[Table(DbConst.table_sys_wallet)]
[Index(nameof(type), IsUnique = true)]
[Index(nameof(wallet_address), IsUnique = true)]
public class SystemWalletModel {
	[Key]
	[Column("id")]
	public int id { get; set; }

	/// [See const] Indentify wallet (for game, stake, leaderboard reward,...)
	/// In general, we use it when compare in query instead of using `wallet_name`.
	[Required]
	[Column("type", TypeName = "tinyint")]
	public SystemWalletModelConst.Type type { get; set; }

	/// Address at Cardano.
	[Required]
	[Column("wallet_address", TypeName = "varchar(255)"), MaxLength(255)]
	public string wallet_address { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }
}
