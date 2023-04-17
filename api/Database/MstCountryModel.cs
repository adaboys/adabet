namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

/// Ref: https://betsapi.com/docs/samples/countries.json
[Table(DbConst.table_mst_country)]
[Index(nameof(code), IsUnique = true)]
public class MstCountryModel : AutoGenerateUpdateTime {
	[Key]
	[Column("id")]
	public int id { get; set; }

	/// Unique country code. For eg,. vn, jp,...
	[Required]
	[Column("code", TypeName = "varchar(10)"), MaxLength(10)]
	public string code { get; set; }

	/// Human readable name of country (Vietnam, 日本, ...)
	[Required]
	[Column("name", TypeName = "nvarchar(255)"), MaxLength(255)]
	public string name { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }
}

public class MstCountryModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<MstCountryModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}
