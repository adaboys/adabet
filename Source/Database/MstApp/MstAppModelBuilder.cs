namespace App;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Tool.Compet.Core;

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
