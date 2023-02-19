namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table(DbConst.table_sport_team)]
public class SportTeamModel : AutoGenerateUpdateTime {
	[Key]
	[Column("id")]
	public long id { get; set; }

	[Required]
	[Column("name_en", TypeName = "varchar(64)"), MaxLength(64)]
	public string name_en { get; set; }

	[Required]
	[Column("name_native", TypeName = "nvarchar(64)"), MaxLength(64)]
	public string name_native { get; set; }

	[Required]
	[Column("flag_image_relative_path", TypeName = "varchar(64)"), MaxLength(64)]
	public string flag_image_relative_path { get; set; }

	/// Reference of onebet team1 id
	[Column("ref_onebet_home_team_id")]
	public long ref_onebet_home_team_id { get; set; }

	/// Reference of onebet team2 id
	[Column("ref_onebet_away_team_id")]
	public long ref_onebet_away_team_id { get; set; }

	/// Reference of away team id from betsapi
	[Column("ref_betsapi_home_team_id", TypeName = "varchar(64)"), MaxLength(64)]
	public string ref_betsapi_home_team_id { get; set; }

	/// Reference of away team id from betsapi
	[Column("ref_betsapi_away_team_id", TypeName = "varchar(64)"), MaxLength(64)]
	public string ref_betsapi_away_team_id { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }
}
