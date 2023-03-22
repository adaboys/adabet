namespace App;

using Microsoft.EntityFrameworkCore;
using Tool.Compet.Core;

public class MstCardanoCoinModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// [Modelizing]
		// Generate default datetime when add.
		modelBuilder.Entity<MstCardanoCoinModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");

		// [Seeding]
		modelBuilder.Entity<MstCardanoCoinModel>().ToTable(DbConst.table_mst_cardano_coin).HasData(
			new MstCardanoCoinModel().AlsoDk(model => {
				model.id = 1;
				model.coin_name = MstCardanoCoinModelConst.COIN_NAME_ADA;
				model.asset_id = MstCardanoCoinModelConst.ASSET_ID_ADA;
			})
		);
	}
}
