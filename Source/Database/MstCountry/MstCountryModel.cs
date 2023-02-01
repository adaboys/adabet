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

	/// Human readable name in native of country (Việt Nam, 日本,...)
	[Required]
	[Column("name_native", TypeName = "nvarchar(64)"), MaxLength(64)]
	public string name_native { get; set; }

	/// Human readable name in english of country (Vietnam, Japan,...)
	[Required]
	[Column("name_en", TypeName = "varchar(64)"), MaxLength(64)]
	public string name_en { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }
}
