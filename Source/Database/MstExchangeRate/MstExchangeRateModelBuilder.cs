namespace App;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Tool.Compet.Core;

public class MstExchangeRateModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<MstExchangeRateModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");

		// [Seeding]
		modelBuilder.Entity<MstExchangeRateModel>().ToTable(DbConst.table_mst_exchange_rate).HasData(
			new MstExchangeRateModel {
				id = 1,
				from_currency = MstExchangeRateModelConst.ADA,
				to_currency = MstExchangeRateModelConst.USD,
				closing_rate = 0.25m,
			},
			new MstExchangeRateModel {
				id = 2,
				from_currency = MstExchangeRateModelConst.USD,
				to_currency = MstExchangeRateModelConst.ADA,
				closing_rate = 4.0m,
			},
			new MstExchangeRateModel {
				id = 3,
				from_currency = MstExchangeRateModelConst.ABE,
				to_currency = MstExchangeRateModelConst.ADA,
				closing_rate = 0.25m,
			},
			new MstExchangeRateModel {
				id = 4,
				from_currency = MstExchangeRateModelConst.ADA,
				to_currency = MstExchangeRateModelConst.ABE,
				closing_rate = 4.0m,
			}
		);
	}
}
