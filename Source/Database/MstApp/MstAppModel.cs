namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

/// Since we have one app run on five OS types.
/// For simple, we just merge them, and consider as `app`.
/// For eg,. app_id: 1 (android), 2 (ios), 3 (webgl), 4 (macos), 5 (windows),...
[Table(DbConst.table_mst_app)]
[Index(nameof(os_type), IsUnique = true)]
public class MstAppModel : AutoGenerateUpdateTime {
	[Key]
	[Column("id")]
	public int id { get; set; }

	/// [See const] Which OS is (android, ios,...)
	[Required]
	[Column("os_type", TypeName = "tinyint")]
	public MstAppModelConst.OsType os_type { get; set; }

	/// App version, for eg,. "2.12.108"
	[Required]
	[Column("app_version", TypeName = "varchar(32)"), MaxLength(32)]
	public string app_version { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }
}
