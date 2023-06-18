namespace App;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class AuthTokenService {
	private readonly AppSetting appSetting;

	public AuthTokenService(
		IOptionsSnapshot<AppSetting> snapshot
	) {
		this.appSetting = snapshot.Value;
	}

	internal string GenerateAccessToken(IEnumerable<Claim> claims) {
		var jwtSection = this.appSetting.jwt;
		var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSection.key));

		var securityToken = new JwtSecurityToken(
			// Server
			issuer: jwtSection.issuer,
			// Client
			audience: jwtSection.audience,
			// We also attach custom claims
			claims: claims,
			// Null expiration means never expired??
			expires: DateTime.UtcNow.AddSeconds(jwtSection.expiresInSeconds),
			// Use `HmacSha256` instead of `HmacSha256Signature` since another services need decode the token
			signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
		);

		return new JwtSecurityTokenHandler().WriteToken(securityToken);
	}

	internal string GenerateRefreshToken(int tokenLength = 40) {
		var guid = Guid.NewGuid().ToStringWithoutHyphen();

		if (tokenLength > guid.Length) {
			// var randomNumber = new byte[tokenLength - guid.Length];
			// using var rng = RandomNumberGenerator.Create();
			// rng.GetBytes(randomNumber);
			// Convert.ToBase64String(randomNumber);

			return guid + CodeGenerator.RandomAlphabets(tokenLength - guid.Length);
		}

		return guid;
	}

	/// Generate unique login_token without special char since it will be displayed in browser.
	internal string GenerateLoginToken(int tokenLength = 40) {
		var guid = Guid.NewGuid().ToStringWithoutHyphen();

		if (tokenLength > guid.Length) {
			var suffix = string.Empty;
			var remainCount = tokenLength - guid.Length;
			while (remainCount-- > 0) {
				suffix += BusinessConst.alphabets[Random.Shared.Next(BusinessConst.alphabets.Length)];
			}

			return guid + suffix;
		}

		return guid;
	}

	/// @param accessToken: Can be expired.
	internal ClaimsPrincipal? GetClaimsPrincipalFromAccessToken(string accessToken) {
		try {
			var tokenValidationParameters = new TokenValidationParameters {
				// You might want to validate the audience and issuer depending on your use case
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.appSetting.jwt.key)),
				// Here we are saying that we don't care about the token's expiration date
				ValidateLifetime = false
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			var claimsPrincipal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
			var validatedToken = securityToken as JwtSecurityToken;

			if (validatedToken == null || !validatedToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
				throw new SecurityTokenException("Invalid token");
			}

			return claimsPrincipal;
		}
		catch {
			return null;
		}
	}

	public async Task<ApiResponse> ValidateAuthToken(string? access_token) {
		// About 2 ms
		if (access_token != null) {
			return await Task.Run<ApiResponse>(() => {
				try {
					var tokenValidationParameters = new TokenValidationParameters {
						// You might want to validate the audience and issuer depending on your use case
						ValidateAudience = false,
						ValidateIssuer = false,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.appSetting.jwt.key)),
						// Here we are saying that we don't care about the token's expiration date
						ValidateLifetime = false
					};
					var tokenHandler = new JwtSecurityTokenHandler();
					var claimsPrincipal = tokenHandler.ValidateToken(access_token, tokenValidationParameters, out var securityToken);
					var validatedToken = securityToken as JwtSecurityToken;

					if (validatedToken == null || !validatedToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
						return new ApiBadRequestResponse("Incorrect token");
					}

					var timeout = validatedToken.ValidTo.Subtract(DateTime.UtcNow).TotalSeconds;

					return new ValidateTokenResponse {
						data = new() {
							expires_in = (int)timeout,
						}
					};
				}
				catch {
					return new ApiBadRequestResponse("Invalid token");
				}
			});
		}

		return new ApiBadRequestResponse("No token for validate");
	}
}
