namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
