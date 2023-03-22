namespace App;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Google.Apis.Auth;
using Tool.Compet.Http;
using Microsoft.EntityFrameworkCore;
using Tool.Compet.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RedLockNet;

/// For 3rd-authentication provider (google, facebook,...)
/// https://developers.google.com/identity/one-tap/android/idtoken-auth
/// https://developers.google.com/api-client-library

/// AccessToken and RefreshToken: https://codepedia.info/aspnet-core-jwt-refresh-token-authentication
public class AuthService : BaseService {
	private readonly ILogger<AuthService> logger;
	private readonly UserDao userDao;
	private readonly ApiNodejsRepo apiNodejsRepo;
	private readonly RedisComponent redisComponent;
	private readonly AuthTokenService authTokenService;
	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly MailComponent mailComponent;
	private readonly UserComponent userComponent;
	private readonly IDistributedLockFactory lockFactory;

	public AuthService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<AuthService> logger,
		UserDao userDao,
		ApiNodejsRepo apiNodejsRepo,
		RedisComponent redisService,
		AuthTokenService tokenService,
		CardanoNodeRepo cardanoNodeRepo,
		MailComponent mailService,
		UserComponent userComponent,
		IDistributedLockFactory lockFactory
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.userDao = userDao;
		this.apiNodejsRepo = apiNodejsRepo;
		this.redisComponent = redisService;
		this.authTokenService = tokenService;
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.mailComponent = mailService;
		this.userComponent = userComponent;
		this.lockFactory = lockFactory;
	}

	public async Task<ApiResponse> LogIn(LoginRequestBody reqBody, string? ipAddress, string? userAgent) {
		var user = await this.userDao.FindValidUserViaEmailAsync(reqBody.email);
		if (user is null) {
			return new ApiUnauthorizedResponse();
		}

		// Allow login normally after a year blocked.
		if (user.login_failed_count >= UserModelConst.MAX_LOGIN_FAILED_COUNT && user.login_denied_at != null) {
			if (user.login_denied_at.Value.AddYears(1) >= DateTime.UtcNow) {
				return new ApiBadRequestResponse { code = ErrCode.blocked };
			}
			user.login_failed_count = 0;
			user.login_denied_at = null;
		}

		// Check password
		if (user.password == null) {
			return new ApiUnauthorizedResponse();
		}
		var passwordHasher = new PasswordHasher<UserModel>();
		if (passwordHasher.VerifyHashedPassword(user, user.password, reqBody.password) == PasswordVerificationResult.Failed) {
			// Remember failed login
			if (++user.login_failed_count >= UserModelConst.MAX_LOGIN_FAILED_COUNT) {
				user.login_denied_at = DateTime.UtcNow;
			}
			await this.dbContext.SaveChangesAsync();

			return new ApiBadRequestResponse {
				code = ErrCode.invalid_attempt,
				data = new Dictionary<string, object> {
					{ "remain_try_count", UserModelConst.MAX_LOGIN_FAILED_COUNT - user.login_failed_count }
				}
			};
		}
		else {
			// Reset failed login if entered correct password.
			user.login_failed_count = 0;
			user.login_denied_at = null;
		}

		return await this._DoLogin(
			loginType: UserAuthModelConst.LoginType.IdPwd,
			clientType: (UserAuthModelConst.ClientType)reqBody.client_type,
			userId: user.id,
			userRole: user.role,
			ipAddress: ipAddress,
			userAgent: userAgent
		);
	}

	/// Create new user if not exist, then Login the user.
	public async Task<ApiResponse> LogInWithProvider(ProviderLoginRequestBody in_reqBody, string? ipAddress, string? userAgent) {
		UserModelConst.SignupType signup_type = 0;
		string? name = null;
		string? email = null;

		switch (in_reqBody.provider) {
			case AppConst.AUTH_PROVIDER_GOOGLE: {
				signup_type = UserModelConst.SignupType.Google;
				(name, email) = await this._GetUserFromGoogle(in_reqBody.id_token, in_reqBody.access_token);
				break;
			}
			case AppConst.AUTH_PROVIDER_FACEBOOK: {
				signup_type = UserModelConst.SignupType.Facebook;
				(name, email) = await this._GetUserFromFacebook(in_reqBody.access_token);
				break;
			}
			default: {
				return new ApiBadRequestResponse();
			}
		}

		if (name is null || email is null) {
			return new ApiBadRequestResponse("Could not get email/name");
		}

		// Create new user if not found email
		var user = await this.dbContext.users.FirstOrDefaultAsync(m => m.email == email);
		if (user is null) {
			user = await this._CreateNewUser(signup_type, name, email);
			if (user is null) {
				return new ApiInternalServerErrorResponse();
			}
		}
		else if (!this.userDao.IsValidUser(user)) {
			return new ApiBadRequestResponse("Invalid user");
		}

		// Perform login
		return await _DoLogin(
			loginType: UserAuthModelConst.LoginType.Provider,
			clientType: in_reqBody.client_type,
			userId: user.id,
			userRole: user.role,
			ipAddress: ipAddress,
			userAgent: userAgent
		);
	}

	public async Task<ApiResponse> LoginWithToken(LoginWithTokenRequestBody requestBody, string? ipAddress, string? userAgent) {
		// Ensure refresh_token is active
		var curAuth = await this.dbContext.userAuths.FirstOrDefaultAsync(m =>
			m.refresh_token == requestBody.refresh_token &&
			m.token_expired_at >= DateTime.UtcNow &&
			m.revoked_at == null
		);
		if (curAuth == null) {
			return new ApiBadRequestResponse();
		}

		// Extract info from access_token
		var claimsPrincipal = this.authTokenService.GetClaimsPrincipalFromAccessToken(requestBody.access_token);
		if (claimsPrincipal == null || claimsPrincipal.Claims == null) {
			return new ApiBadRequestResponse();
		}

		var claimUserId = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == AppConst.jwt.claim_type_user_id)?.Value;
		var claimClientType = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == AppConst.jwt.claim_type_client_type)?.Value;

		// Parse user_id
		if (!Guid.TryParse(claimUserId, out var user_id)) {
			return new ApiBadRequestResponse();
		}

		// Check user
		if (await this.userDao.InvalidUserById(user_id)) {
			return new ApiBadRequestResponse();
		}

		// Replace client_type from current access_token
		var newClaims = new List<Claim>();
		foreach (var claim in claimsPrincipal.Claims) {
			if (claim.Type == AppConst.jwt.claim_type_client_type) {
				newClaims.Add(new Claim(AppConst.jwt.claim_type_client_type, requestBody.client_type.ToString()));
			}
			else {
				newClaims.Add(claim);
			}
		}

		return await this._DoLoginWithClaims(
			loginType: UserAuthModelConst.LoginType.Token,
			clientType: (UserAuthModelConst.ClientType)requestBody.client_type,
			userId: user_id,
			claims: newClaims,
			ipAddress: ipAddress,
			userAgent: userAgent
		);
	}

	/// Just response signature (description + nonce)
	public async Task<ApiResponse> RequestLoginWithExternalWallet(RequestLoginWithExternalWalletRequestBody in_reqBody) {
		var external_wallet_address = in_reqBody.wallet_address;

		// Must match with env
		if (!CardanoHelper.IsAddressMatchWithEnv(this.appSetting, external_wallet_address)) {
			return new ApiBadRequestResponse("Address must match with current env") { code = ErrCode.invalid_address };
		}
		// Validate address
		var validateRepsonse = await this.cardanoNodeRepo.ValidateAddressAsync(external_wallet_address);
		if (validateRepsonse.failed) {
			return new ApiFailureResponse(validateRepsonse.status, "Invalid address") { code = validateRepsonse.code };
		}

		// Cache nonce for the address for later re-build signature
		var cache = new LoginWithExternalWalletCache {
			nonce = CryptoHelper.GenerateNonce()
		};
		var cacheKey = RedisKey.ForLoginWithExternalWallet(external_wallet_address);
		var cached = await this.redisComponent.SetJsonAsync(cacheKey, cache, TimeSpan.FromMinutes(30));
		if (!cached) {
			return new ApiInternalServerErrorResponse("Could not generate nonce");
		}

		return new RequestLoginWithExternalWalletResponse {
			data = new() {
				signature = CryptoHelper.MakeRequestSignatureInHex(cache.nonce)
			}
		};
	}

	/// Perform login
	public async Task<ApiResponse> LoginWithExternalWallet(LoginWithExternalWalletRequestBody reqBody, string? ipAddress, string? userAgent) {
		var external_wallet_address = reqBody.wallet_address;

		// Requires nonce
		var cacheKey = RedisKey.ForLoginWithExternalWallet(external_wallet_address);
		var cache = await this.redisComponent.GetJsonAsync<LoginWithExternalWalletCache>(cacheKey);
		if (cache is null) {
			return new ApiBadRequestResponse("Login expired");
		}

		// Verify ownership of the address
		var verifyAddressResponse = await this.apiNodejsRepo.VerifyAddressAsync(
			address: external_wallet_address,
			address_in_hex: reqBody.wallet_address_in_hex,
			request_signature: CryptoHelper.MakeRequestSignatureInHex(cache.nonce),
			response_signature: reqBody.signed_signature,
			response_key: reqBody.signed_key
		);
		if (verifyAddressResponse.failed) {
			return new ApiBadRequestResponse("Unmatched ownership");
		}

		UserModel? user = null;

		var wallet = await this.dbContext.userWallets.FirstOrDefaultAsync(m => m.wallet_address == external_wallet_address);

		// Find user
		if (wallet != null && wallet.deleted_at == null) {
			user = await this.userDao.FindValidUserViaIdAsync(wallet.user_id);
		}
		// Create new user flow
		else {
			// Step 1. Need email
			if (reqBody.email is null) {
				return new ApiBadRequestResponse("Email required") { code = ErrCode.email_required };
			}
			// Step 2. Send otp
			if (reqBody.request_otp) {
				cache.email = reqBody.email;
				cache.otp = CodeGenerator.GenerateOtpCode();

				const int timeout = 15;

				var sendMailResponse = await this.mailComponent.SendAsync(
					toEmail: reqBody.email,
					subject: "Register confirmation",
					body: await MailTemplate.ForAttemptRegisterUser(cache.otp, timeout)
				);
				if (sendMailResponse.failed) {
					return sendMailResponse;
				}

				var cached = await this.redisComponent.SetJsonAsync(cacheKey, cache, TimeSpan.FromMinutes(timeout));
				if (!cached) {
					return new ApiInternalServerErrorResponse("Could not generate otp");
				}

				return new ApiSuccessResponse("Check mail for otp");
			}
			// Step 3. Need otp
			if (reqBody.otp is null) {
				return new ApiBadRequestResponse("OTP required") { code = ErrCode.otp_required };
			}

			// Step 4. Flow done, check and goto create new user to login
			if (reqBody.email != cache.email || reqBody.otp != cache.otp) {
				return new ApiBadRequestResponse("Invalid email/otp") { code = ErrCode.invalid_data };
			}
			if (reqBody.wallet_name is null) {
				return new ApiBadRequestResponse("Pls provide wallet name");
			}

			// OK, create new user or link wallet with lock support of redis
			user = await this._CreateNewUserOrLinkWalletWhenLoginWithExternalWalletAsync(
				email: reqBody.email,
				wallet_address: external_wallet_address,
				wallet_name: reqBody.wallet_name
			);
		}

		if (user is null) {
			return new ApiBadRequestResponse("Invalid user");
		}

		// OK, delete cache
		await this.redisComponent.DeleteKeyAsync(cacheKey);

		// Perform login
		return await _DoLogin(
			loginType: UserAuthModelConst.LoginType.ExternalWallet,
			clientType: reqBody.client_type,
			userId: user.id,
			userRole: user.role,
			ipAddress: ipAddress,
			userAgent: userAgent
		);
	}

	private async Task<UserModel?> _CreateNewUserOrLinkWalletWhenLoginWithExternalWalletAsync(
		string email,
		string wallet_address,
		string wallet_name
	) {
		var resource = RedisKey.LockForUserRegistration(email);
		var expiry = TimeSpan.FromMinutes(10);

		using (var redLock = await this.lockFactory.CreateLockAsync(resource, expiry)) {
			if (redLock.IsAcquired) {
				using (var transaction = await this.dbContext.Database.BeginTransactionAsync()) {
					try {
						// Get or Create user
						var user = await this.dbContext.users.FirstOrDefaultAsync(m => m.email == email);
						if (user is null) {
							user = await this.userComponent.CreateUserOrThrowAsync(
								role: UserModelConst.Role.User,
								signup_type: UserModelConst.SignupType.ExternalWallet,
								name: null,
								email: email,
								password: null
							);
						}

						// Create or Link external wallet.
						// Since we can soft-delete a wallet, so always update
						// data when create or link the wallet.
						var wallet = await this.dbContext.userWallets.FirstOrDefaultAsync(m => m.wallet_address == wallet_address);
						if (wallet is null) {
							wallet = new() {
								wallet_address = wallet_address,
								wallet_type = UserWalletModelConst.WalletType.External,
								wallet_status = UserWalletModelConst.WalletStatus.Active,
							};

							this.dbContext.userWallets.Attach(wallet);
						}

						wallet.user_id = user.id;
						wallet.wallet_name = wallet_name;
						wallet.deleted_at = null;

						await this.dbContext.SaveChangesAsync();
						await transaction.CommitAsync();

						return user;
					}
					catch (Exception e) {
						this.logger.ErrorDk(this, "Could not create new user when login with external wallet, data: {@data}", new Dictionary<string, object> {
							{ "email", email },
							{ "wallet_address", wallet_address },
							{ "wallet_name", wallet_name },
							{ "error", e.Message }
						});

						await transaction.RollbackAsync();
					}
				}
			}
		}

		return null;
	}

	/// Refresh access token if it is not yet expired.
	/// After that, generate new {accessToken, refreshToken}, and reset refreshToken expiry time.
	public async Task<ApiResponse> SilentLogin(SilentLoginRequestBody requestBody, string? ipAddress, string? userAgent) {
		var claimsPrincipal = this.authTokenService.GetClaimsPrincipalFromAccessToken(requestBody.access_token);
		if (claimsPrincipal == null || claimsPrincipal.Claims == null) {
			return new ApiUnauthorizedResponse();
		}

		var claimUserId = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == AppConst.jwt.claim_type_user_id)?.Value;
		var claimClientType = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == AppConst.jwt.claim_type_client_type)?.Value;
		var clientType = (UserAuthModelConst.ClientType)claimClientType.ParseByteDk();

		if (Guid.TryParse(claimUserId, out var userId)) {
			// Check user
			if (await this.userDao.InvalidUserById(userId)) {
				return new ApiUnauthorizedResponse();
			}

			// Revoke current login for next login session !
			var curAuth = await this.dbContext.userAuths.FirstOrDefaultAsync(m =>
				m.user_id == userId &&
				m.refresh_token == requestBody.refresh_token &&
				m.token_expired_at >= DateTime.UtcNow &&
				m.revoked_at == null
			);
			if (curAuth != null) {
				curAuth.revoked_at = DateTime.UtcNow;
				curAuth.revoked_by_ip = ipAddress;
				curAuth.revoked_by_token = requestBody.refresh_token;
				curAuth.revoked_by_agent = userAgent;

				return await this._DoLoginWithClaims(
					loginType: UserAuthModelConst.LoginType.Silent,
					clientType: clientType,
					userId: userId,
					claims: claimsPrincipal.Claims,
					ipAddress: ipAddress,
					userAgent: userAgent
				);
			}
		}

		// Client must be re-login (by client manually)
		return new ApiUnauthorizedResponse();
	}

	public async Task<ApiResponse> LogOut(Guid userId, LogoutRequestBody requestBody, string? ipAddress, string? userAgent) {
		var user = await this.dbContext.users.FirstOrDefaultAsync(m => m.id == userId);
		if (user == null) {
			return new ApiBadRequestResponse();
		}

		// Revoke all accesses (even though tokens are active).
		var query = this.dbContext.userAuths.Where(m => m.revoked_at == null && m.user_id == user.id);
		if (!requestBody.logout_everywhere) {
			query = query.Where(m => m.refresh_token == requestBody.refresh_token);
		}

		var accesses = await query.ToArrayAsync();
		if (accesses != null && accesses.Length > 0) {
			foreach (var access in accesses) {
				access.revoked_at = DateTime.UtcNow;
				access.revoked_by_token = requestBody.refresh_token;
				access.revoked_by_ip = ipAddress;
				access.revoked_by_agent = userAgent;
			}
			await this.dbContext.SaveChangesAsync();

			return new ApiSuccessResponse("Logged out" + (requestBody.logout_everywhere ? " everywhere" : ""));
		}

		return new ApiSuccessResponse("Nothing affected");
	}

	private async Task<ApiResponse> _DoLogin(
		UserAuthModelConst.LoginType loginType,
		UserAuthModelConst.ClientType clientType,
		Guid userId,
		UserModelConst.Role userRole,
		string? ipAddress,
		string? userAgent
	) {
		var claims = new[] {
			// Subject of the site
			new Claim(JwtRegisteredClaimNames.Sub, this.appSetting.jwt.subject),
			// This is jwt id (should different between issuers)
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToStringDk()),
			// Issued at (must be UtcNow from 1970)
			new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
			// Attach our custom claims
			new Claim(AppConst.jwt.claim_type_user_id, userId.ToStringDk()),
			new Claim(AppConst.jwt.claim_type_user_role, ((int)userRole).ToString()),
			new Claim(AppConst.jwt.claim_type_client_type, ((int)clientType).ToString()),
		};

		return await this._DoLoginWithClaims(
			loginType: loginType,
			clientType: clientType,
			userId: userId,
			claims: claims,
			ipAddress: ipAddress,
			userAgent: userAgent
		);
	}

	private async Task<ApiResponse> _DoLoginWithClaims(
		UserAuthModelConst.LoginType loginType,
		UserAuthModelConst.ClientType clientType,
		Guid userId,
		IEnumerable<Claim> claims,
		string? ipAddress,
		string? userAgent
	) {
		var refreshToken = this.authTokenService.GenerateRefreshToken();
		var accessToken = this.authTokenService.GenerateAccessToken(claims);

		try {
			// Add login info
			this.dbContext.userAuths.Attach(new() {
				user_id = userId,
				login_type = loginType,
				client_type = clientType,
				refresh_token = refreshToken,
				token_expired_at = DateTime.UtcNow.AddSeconds(this.appSetting.jwt.refreshExpiresInSeconds),
				created_by_ip = ipAddress,
				created_by_agent = userAgent,
			});

			await this.dbContext.SaveChangesAsync();

			return new LoginResponse {
				data = new() {
					token_schema = "Bearer",
					access_token = accessToken,
					refresh_token = refreshToken
				}
			};
		}
		catch (Exception e) {
			this.logger.ErrorDk(this, "Could not login, data: {@data}", new Dictionary<string, object> {
				{ "error", e.Message },
				{ "clientType" , clientType },
				{ "loginType", loginType },
				{ "userId", userId },
			});

			return new ApiInternalServerErrorResponse();
		}
	}

	/// @param idToken: This is jwt.
	/// @param accessToken: This is jwt.
	/// @returns (name, email)
	private async Task<(string?, string?)> _GetUserFromGoogle(string? idToken, string? accessToken) {
		try {
			if (idToken != null) {
				var response = await GoogleJsonWebSignature.ValidateAsync(idToken);
				return (response.Name, response.Email);
			}

			if (accessToken != null) {
				// For tokeninfo: var url = $"https://www.googleapis.com/oauth2/v3/tokeninfo?access_token={accessToken}";
				var url = $"https://www.googleapis.com/oauth2/v3/userinfo?access_token={accessToken}";
				var response = await new DkHttpClient().GetForType<GoogleAccessTokenDecodingResponse>(url);

				return (response?.name, response?.email);
			}

			return (null, null);
		}
		catch {
			return (null, null);
		}
	}

	/// See: https://developers.facebook.com/tools
	/// Entry: Tools -> Graph API Explorer
	/// @returns (name, email)
	private async Task<(string?, string?)> _GetUserFromFacebook(string? accessToken) {
		try {
			if (accessToken != null) {
				var url = $"https://graph.facebook.com/me?fields=name,email&access_token={accessToken}";
				var response = await new DkHttpClient().GetForType<FacebookAccessTokenDecodingResponse>(url);

				return (response?.name, response?.email);
			}

			return (null, null);
		}
		catch {
			return (null, null);
		}
	}

	private async Task<UserModel?> _CreateNewUser(UserModelConst.SignupType signup_type, string name, string email) {
		// Start transaction.
		// Ref: https://docs.microsoft.com/en-us/ef/core/saving/transactions
		using var transaction = await this.dbContext.Database.BeginTransactionAsync();

		try {
			// Create new user with extra work
			var newUser = await userComponent.CreateUserOrThrowAsync(
				role: UserModelConst.Role.User,
				signup_type: signup_type,
				name: name,
				email: email
			);

			// Save changes and Commit
			await this.dbContext.SaveChangesAsync();
			await transaction.CommitAsync();

			return newUser;
		}
		catch (Exception e) {
			this.logger.CriticalDk(this,
				"Could not create new user from email: {email}, error: {Message}",
				email,
				e.Message
			);

			await transaction.RollbackAsync();

			return null;
		}
	}

	public async Task<ApiResponse> RequestResetPassword(string email) {
		if (await this.userDao.InvalidUserByEmail(email)) {
			return new ApiBadRequestResponse("Invalid user email");
		}

		const int timeout = 30;
		var reset_password_token = CodeGenerator.GenerateOtpCode(80);

		// Cache token
		var cacheKey = RedisKey.ForResetPassword(email);
		var cached = await this.redisComponent.SetStringAsync(cacheKey, reset_password_token, TimeSpan.FromMinutes(timeout));
		if (!cached) {
			return new ApiInternalServerErrorResponse("Could not generate token");
		}

		// Send mail
		var sendMailResponse = await this.mailComponent.SendAsync(
			email,
			"Reset password",
			await MailTemplate.ForResetPassword(reset_password_token, timeout)
		);
		if (sendMailResponse.failed) {
			return new ApiInternalServerErrorResponse("Could not send mail");
		}

		return new ApiSuccessResponse();
	}

	public async Task<ApiResponse> ConfirmResetPassword(ConfirmResetPasswordRequestBody requestBody, string? ipAddress, string? userAgent) {
		var email = requestBody.email;

		// Check OTP
		var cacheKey = RedisKey.ForResetPassword(email);
		var reset_password_token = await this.redisComponent.GetStringAsync(cacheKey);
		if (reset_password_token != requestBody.otp_code) {
			return new ApiBadRequestResponse("Invalid OTP") { code = ErrCode.invalid_otp };
		}

		// Delete OTP cache
		await this.redisComponent.DeleteKeyAsync(cacheKey);

		// Change password
		var user = await this.userDao.FindValidUserViaEmailAsync(email);
		if (user is null) {
			return new ApiBadRequestResponse("Invalid user");
		}
		var passwordHasher = new PasswordHasher<UserModel>();
		user.password = passwordHasher.HashPassword(user, requestBody.new_pass);

		// Logout from everywhere
		if (requestBody.logout_everywhere) {
			var userAuths = await this.dbContext.userAuths
				.Where(m => m.user_id == user.id && m.revoked_at == null)
				.ToArrayAsync()
			;
			var now = DateTime.UtcNow;
			foreach (var auth in userAuths) {
				auth.revoked_at = now;
				auth.revoked_by_ip = ipAddress;
				auth.revoked_by_agent = userAgent;
			}
		}

		await this.dbContext.SaveChangesAsync();

		return new ApiSuccessResponse();
	}

	private sealed class LoginWithExternalWalletCache {
		public string nonce { get; set; }
		public string email { get; set; }
		public string otp { get; set; }
	}
}
