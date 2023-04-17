namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tool.Compet.Core;

/// Sports (football, badminton, volleyball,...)
[Table(DbConst.table_mst_sport)]
public class MstSportModel {
	[Key]
	[Column("id")]
	public int id { get; set; }

	/// Human readable name of sport (football, badminton, table tennis,...)
	[Required]
	[Column("name", TypeName = "varchar(64)"), MaxLength(64)]
	public string name { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }
}

public class MstSportModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// [Modelizing]
		// Generate default datetime when add.
		modelBuilder.Entity<MstSportModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");

		// [Seeding]
		modelBuilder.Entity<MstSportModel>().ToTable(DbConst.table_mst_sport).HasData(
			new MstSportModel().AlsoDk(model => {
				model.id = MstSportModelConst.Id_Football;
				model.name = MstSportModelConst.Name_Football;
			}),
			new MstSportModel().AlsoDk(model => {
				model.id = MstSportModelConst.Id_Tennis;
				model.name = MstSportModelConst.Name_Tennis;
			}),
			new MstSportModel().AlsoDk(model => {
				model.id = MstSportModelConst.Id_Basketball;
				model.name = MstSportModelConst.Name_Basketball;
			}),
			new MstSportModel().AlsoDk(model => {
				model.id = MstSportModelConst.Id_Volleyball;
				model.name = MstSportModelConst.Name_Volleyball;
			}),
			new MstSportModel().AlsoDk(model => {
				model.id = MstSportModelConst.Id_TableTennis;
				model.name = MstSportModelConst.Name_TableTennis;
			}),
			new MstSportModel().AlsoDk(model => {
				model.id = MstSportModelConst.Id_Boxing;
				model.name = MstSportModelConst.Name_Boxing;
			}),
			new MstSportModel().AlsoDk(model => {
				model.id = MstSportModelConst.Id_MMA;
				model.name = MstSportModelConst.Name_MMA;
			})
		);
	}
}

public class MstSportModelConst {
	public const int Id_Football = 1;
	public const string Name_Football = "Football";

	public const int Id_Tennis = 2;
	public const string Name_Tennis = "Tennis";

	public const int Id_Basketball = 3;
	public const string Name_Basketball = "Basketball";

	public const int Id_Volleyball = 4;
	public const string Name_Volleyball = "Volleyball";

	public const int Id_TableTennis = 5;
	public const string Name_TableTennis = "TableTennis";

	public const int Id_Boxing = 6;
	public const string Name_Boxing = "Boxing";

	public const int Id_MMA = 7;
	public const string Name_MMA = "MMA";
}
