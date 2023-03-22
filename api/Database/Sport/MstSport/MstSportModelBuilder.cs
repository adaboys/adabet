namespace App;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Tool.Compet.Core;

public class MstSportModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// [Modelizing]
		// Generate default datetime when add.
		modelBuilder.Entity<MstSportModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");

		// [Seeding]
		modelBuilder.Entity<MstSportModel>().ToTable(DbConst.table_mst_sport).HasData(
			new MstSportModel().AlsoDk(model => {
				model.id = 1;
				model.name = MstSportModelConst.Name_Football;
			}),
			new MstSportModel().AlsoDk(model => {
				model.id = 2;
				model.name = MstSportModelConst.Name_Tennis;
			}),
			new MstSportModel().AlsoDk(model => {
				model.id = 3;
				model.name = MstSportModelConst.Name_Basketball;
			}),
			new MstSportModel().AlsoDk(model => {
				model.id = 4;
				model.name = MstSportModelConst.Name_Volleyball;
			}),
			new MstSportModel().AlsoDk(model => {
				model.id = 5;
				model.name = MstSportModelConst.Name_TableTennis;
			}),
			new MstSportModel().AlsoDk(model => {
				model.id = 6;
				model.name = MstSportModelConst.Name_Boxing;
			}),
			new MstSportModel().AlsoDk(model => {
				model.id = 7;
				model.name = MstSportModelConst.Name_MMA;
			})
		);
	}
}
