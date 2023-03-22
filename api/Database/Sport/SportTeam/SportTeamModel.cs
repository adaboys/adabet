namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table(DbConst.table_sport_team)]
public class SportTeamModel : AutoGenerateUpdateTime {
	[Key]
	[Column("id")]
	public long id { get; set; }

	/// Team name, for eg,. 札幌楽勝
	[Required]
	[Column("name", TypeName = "nvarchar(64)"), MaxLength(64)]
	public string name { get; set; }

	/// Couple with `flag_image_src`. See: https://betsapi.com/docs/events/faq.html
	/// When image from betsapi, the image path is formed as:
	/// - small: https://assets.b365api.com/images/team/s/921312.png
	/// - medium: https://assets.b365api.com/images/team/m/921312.png
	/// - big: https://assets.b365api.com/images/team/b/921312.png
	[Column("flag_image_name", TypeName = "varchar(64)"), MaxLength(64)]
	public string? flag_image_name { get; set; }

	[Column("flag_image_src", TypeName = "tinyint")]
	public SportTeamModelConst.FlagImageSource flag_image_src { get; set; }

	/// Reference of away team id from betsapi
	[Column("ref_betsapi_home_team_id")]
	public long ref_betsapi_home_team_id { get; set; }

	/// Reference of away team id from betsapi
	[Column("ref_betsapi_away_team_id")]
	public long ref_betsapi_away_team_id { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }
}
