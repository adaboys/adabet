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

public class MstAppModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// [Modelizing]
		// Generate default datetime when add.
		modelBuilder.Entity<MstAppModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");

		// [Seeding]
		modelBuilder.Entity<MstAppModel>().ToTable(DbConst.table_mst_app).HasData(
			new MstAppModel {
				id = 1,
				os_type = MstAppModelConst.OsType.Android,
				app_version = "1.0.0",
			},
			new MstAppModel {
				id = 2,
				os_type = MstAppModelConst.OsType.Ios,
				app_version = "1.0.0",
			},
			new MstAppModel {
				id = 3,
				os_type = MstAppModelConst.OsType.Web,
				app_version = "1.0.0",
			},
			new MstAppModel {
				id = 4,
				os_type = MstAppModelConst.OsType.Macos,
				app_version = "1.0.0",
			},
			new MstAppModel {
				id = 5,
				os_type = MstAppModelConst.OsType.Windows,
				app_version = "1.0.0",
			}
		);
	}
}

public class MstAppModelConst {
	public enum OsType {
		Android = 1,
		Ios = 2,
		Web = 3,
		Macos = 4,
		Windows = 5,
	}
}
