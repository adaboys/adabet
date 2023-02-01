namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

/// Since we cannot revoke access_token after generated.
/// So we introduce this table to handle/revoke auth of an user
/// by revoke refresh_token which we issued to the user.
///
/// Note that, when revoke the auth, then user cannot use that refresh_token more,
/// so that user cannot perform silentLogin, logout, loginWithToken,... with that refresh_token.
///
/// But for apis that use only access_token, the user can still access normally !
[Table(DbConst.table_user_auth)]
[Index(nameof(user_id), nameof(refresh_token))]
public class UserAuthModel : AutoGenerateUpdateTime {
	[Key]
	[Column("id")]
	public long id { get; set; }

	/// Point to field `user.id`.
	[ForeignKey(DbConst.table_user)]
	[Column("user_id")]
	public Guid user_id { get; set; }

	/// In general, this is unique, but should combine with user_id to conduct a PK.
	[Required]
	[Column("refresh_token", TypeName = "varchar(255)"), MaxLength(255)]
	public string refresh_token { get; set; }

	/// When the refresh_token expires.
	[Required]
	[Column("token_expired_at")]
	public DateTime token_expired_at { get; set; }

	/// [See const] Indicates which way (id/pwd, provider, token,...) that user has used to login.
	[Required]
	[Column("login_type", TypeName = "tinyint")]
	public UserAuthModelConst.LoginType login_type { get; set; }

	/// [See const] Indicates about client environment (android, ios, webgl, marketplace,...) that be used for login.
	[Required]
	[Column("client_type", TypeName = "tinyint")]
	public UserAuthModelConst.ClientType client_type { get; set; }

	/// Ip of the client who creates this auth
	[Column("created_by_ip", TypeName = "varchar(64)"), MaxLength(64)]
	public string? created_by_ip { get; set; }

	/// UserAgent of the client who creates this auth
	[Column("created_by_agent", TypeName = "varchar(255)"), MaxLength(255)]
	public string? created_by_agent { get; set; }

	/// This is same as deleted_at, but is set when user want to revoke access (for eg,. log out, silent login,...).
	[Column("revoked_at")]
	public DateTime? revoked_at { get; set; }

	/// Ip of the client who revokes this auth
	[Column("revoked_by_ip", TypeName = "varchar(64)"), MaxLength(64)]
	public string? revoked_by_ip { get; set; }

	/// UserAgent of the client who revokes this auth
	[Column("revoked_by_agent", TypeName = "varchar(255)"), MaxLength(255)]
	public string? revoked_by_agent { get; set; }

	[Column("revoked_by_token", TypeName = "varchar(255)"), MaxLength(255)]
	public string? revoked_by_token { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }

	/// FK models (property name must be same with table name)
	public UserModel user { get; set; }
}
