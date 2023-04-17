namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

/// Exchange-rate of currencies.
/// Formulas: closing_rate = from_currency / to_currency.
/// For eg,. if closing_rate of ADA/USD is 0.402, then 1 ADA = 0.402 USD, or 100 ADA = 40.2 USD.
/// Ref: https://dba.stackexchange.com/questions/221182/database-table-for-exchange-rates
[Table(DbConst.table_mst_exchange_rate)]
[Index(nameof(from_currency), nameof(to_currency))]
public class MstExchangeRateModel : AutoGenerateUpdateTime {
	[Key]
	[Column("id")]
	public int id { get; set; }

	/// For eg,. USD, ADA
	[Required]
	[Column("from_currency", TypeName = "varchar(32)"), MaxLength(32)]
	public string from_currency { get; set; }

	/// For eg,. ADA, ABE
	[Required]
	[Column("to_currency", TypeName = "varchar(32)"), MaxLength(32)]
	public string to_currency { get; set; }

	/// Final rate.
	/// Formulas: from_currency = closing_rate * to_currency
	[Required]
	[Column("closing_rate", TypeName = "decimal(19,9)")]
	public decimal closing_rate { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }
}

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

public class MstExchangeRateModelConst {
	/// List of currency.
	public const string USD = "USD";
	public const string ADA = "ADA";
	public const string ABE = "ABE";
}
