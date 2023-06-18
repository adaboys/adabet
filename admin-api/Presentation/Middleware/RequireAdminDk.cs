namespace App;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Tool.Compet.Core;

/// Annotate with this to prevent access from under-admin.
/// This is equivalented to Authorize + RequireAdmin.
public class RequireAdminDk : AuthorizeAttribute, IAuthorizationFilter {
	public void OnAuthorization(AuthorizationFilterContext context) {
		try {
			// For JWT decoding
			var claims = context.HttpContext.User.Claims;
			var userRole = claims.FirstOrDefault(claim => claim.Type == AppConst.jwt.claim_type_user_role)?.Value;

			if (userRole.ParseIntDk() < (int)(UserModelConst.Role.Admin)) {
				context.Result = new ForbidResult();
			}
		}
		catch {
			context.Result = new UnauthorizedResult();
		}
	}
}
