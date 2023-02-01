namespace App;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// League is group of matches. So, each match should be in a league.
[Table(DbConst.table_sport_league)]
public class SportLeagueModel : AutoGenerateUpdateTime {
	internal string ref_betsapi_league_id;

	[Key]
	[Column("id")]
	public long id { get; set; }

	/// Which sport (football, tennis, pingpong,...)
	[ForeignKey(DbConst.table_mst_sport)]
	[Column("sport_id")]
	public int sport_id { get; set; }

	/// Which country that the league is belong.
	/// It will be NULL (for eg,. world cup) if the league does not belong to any country.
	[ForeignKey(DbConst.table_mst_country)]
	[Column("country_id")]
	public int? country_id { get; set; }

	/// Human readable name of league (V-league,...)
	[Required]
	[Column("name", TypeName = "nvarchar(255)"), MaxLength(255)]
	public string name { get; set; }

	[Column("ref_onebet_league_id")]
	public long ref_onebet_league_id { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }

	/// FK models (property name must be same with table name)
	public MstSportModel mst_sport { get; set; }
}
