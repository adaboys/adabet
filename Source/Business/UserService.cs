namespace App;

using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RedLockNet;

/// Raw query with interpolated: https://docs.microsoft.com/en-us/ef/core/querying/raw-sql
public class UserService : BaseService {
	private readonly ILogger<UserService> logger;
	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly UserDao userDao;
	private readonly MailComponent mailService;
	private readonly UserComponent userComponent;
	private readonly RedisComponent redisComponent;
	private readonly IDistributedLockFactory lockFactory;

	public UserService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UserService> logger,
		CardanoNodeRepo cardanoNodeRepo,
		UserDao userDao,
		MailComponent mailService,
		UserComponent userComponent,
		RedisComponent redisComponent,
		IDistributedLockFactory lockFactory
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.userDao = userDao;
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.mailService = mailService;
		this.userComponent = userComponent;
		this.redisComponent = redisComponent;
		this.lockFactory = lockFactory;
	}

	public async Task<ApiResponse> GetUserProfile(Guid userId) {
		var query =
			from _user in this.dbContext.users
			join _wallet in this.dbContext.userWallets on _user.id equals _wallet.user_id

			where _user.id == userId && _wallet.wallet_type == UserWalletModelConst.WalletType.Internal

			select new {
				_email = _user.email,
				_wallet_address = _wallet.wallet_address,
				_has_password = _user.password != null
			}
		;
		var result = await query.FirstOrDefaultAsync();
		if (result == null) {
			return new ApiBadRequestResponse("Not found user/wallet");
		}

		return new GetUserProfileResponse {
			data = new() {
				email = result._email,
				wallet_address = result._wallet_address,
				has_password = result._has_password,
			}
		};
	}

	public async Task<ApiResponse> GetUserBalance(Guid userId) {
		var query =
			from _user in this.dbContext.users
			join _wallet in this.dbContext.userWallets on _user.id equals _wallet.user_id

			where _user.id == userId && _wallet.wallet_type == UserWalletModelConst.WalletType.Internal

			select new {
				_wallet_address = _wallet.wallet_address,
			}
		;
		var result = await query.FirstOrDefaultAsync();
		if (result == null) {
			return new ApiBadRequestResponse("No user/wallet");
		}

		// Query balance from internal wallet
		var ada_balance = 0m;

		var assetsResponse = await this.cardanoNodeRepo.GetMergedAssetsAsync(result._wallet_address);
		if (assetsResponse.succeed) {
			ada_balance = CardanoHelper.CalcTotalAdaFromAssets(assetsResponse.data.assets);
		}

		return new GetUserBalanceResponse {
			data = new() {
				ada_balance = ada_balance,
			}
		};
	}

	public async Task<ApiResponse> ChangePassword(Guid userId, ChangePasswordRequestBody requestBody, string? ipAddress, string? userAgent) {
		var user = await this.userDao.FindValidUserViaIdAsync(userId);
		if (user == null) {
			return new ApiBadRequestResponse("No user");
		}

		var passwordHasher = new PasswordHasher<UserModel>();
		if (user.password != null) {
			if (requestBody.current_pass is null) {
				return new ApiBadRequestResponse("Current pass must not be empty");
			}
			if (passwordHasher.VerifyHashedPassword(user, user.password, requestBody.current_pass) == PasswordVerificationResult.Failed) {
				return new ApiBadRequestResponse("Incorrect current pass") { code = ErrCode.cur_pass_not_match };
			}
		}

		// Change password
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
				auth.revoked_by_token = auth.refresh_token;
				auth.revoked_by_ip = ipAddress;
				auth.revoked_by_agent = userAgent;
			}
		}

		// Mail to user
		var mailResponse = await this.mailService.SendAsync(
			user.email,
			"Password was changed",
			await MailTemplate.ForChangePassword()
		);
		if (mailResponse.failed) {
			return mailResponse;
		}

		await this.dbContext.SaveChangesAsync();

		return new ApiSuccessResponse("Password changed");
	}

	public async Task<ApiResponse> UpdateUserIdentity(Guid userId, UpdateUserIdentityRequestBody requestBody) {
		var user = await this.userDao.FindValidUserViaIdAsync(userId);
		if (user == null) {
			return new ApiNotFoundResponse();
		}

		user.name = requestBody.full_name.Trim();
		user.telno = requestBody.telno.Trim();

		await this.dbContext.SaveChangesAsync();

		return new UpdateUserIdentityResponse {
			data = new() {
				full_name = user.name,
				telno = user.telno,
			}
		};
	}

	public async Task<ApiResponse> AttemptRegisterUser(AttemptRegisterUserRequestBody reqBody) {
		var user = await this.dbContext.users.FirstOrDefaultAsync(m => m.email == reqBody.email);
		if (user != null) {
			return new ApiBadRequestResponse { code = ErrCode.user_existed };
		}

		// Check to avoid multiple attempts
		var cacheKey = RedisKey.ForUserRegistration(reqBody.email);
		var registry = await this.redisComponent.GetJsonAsync<UserRegistryCache>(cacheKey);
		if (registry != null) {
			return new ApiBadRequestResponse("Already requested registration") { code = ErrCode.duplicated_register };
		}

		// Send OTP
		const int timeout = 15;
		var otpCode = CodeGenerator.GenerateOtpCode();
		var sendMailResponse = await this.mailService.SendAsync(
			toEmail: reqBody.email,
			subject: "Register Confirmation",
			body: await MailTemplate.ForAttemptRegisterUser(otpCode, timeout)
		);
		if (sendMailResponse.failed) {
			return new ApiInternalServerErrorResponse(this.appSetting.debug ? sendMailResponse.message : "Could not send mail");
		}

		// Cache registry for use at next confirm step
		var passwordHasher = new PasswordHasher<UserRegistryCache>();
		var registryCache = new UserRegistryCache {
			name = reqBody.name,
			email = reqBody.email,
			otp_code = otpCode,
		};
		registryCache.hashed_password = passwordHasher.HashPassword(registryCache, reqBody.password);

		await this.redisComponent.SetJsonAsync(cacheKey, registryCache, TimeSpan.FromMinutes(timeout));

		return new ApiSuccessResponse();
	}

	public async Task<ApiResponse> ConfirmRegisterUser(ConfirmRegisterUserRequestBody in_reqBody) {
		var cacheKey = RedisKey.ForUserRegistration(in_reqBody.email);
		var registryCache = await this.redisComponent.GetJsonAsync<UserRegistryCache>(cacheKey);
		if (registryCache is null || registryCache.otp_code != in_reqBody.otp_code) {
			return new ApiBadRequestResponse("Invalid registration") { code = ErrCode.invalid_register };
		}

		// Try to lock on the redis key
		var resource = RedisKey.LockForUserRegistration(in_reqBody.email);
		var expiry = TimeSpan.FromMinutes(2);

		using (var redLock = await this.lockFactory.CreateLockAsync(resource, expiry)) {
			if (redLock.IsAcquired) {
				// We expect cache is alive after acquired the lock
				registryCache = await this.redisComponent.GetJsonAsync<UserRegistryCache>(cacheKey);
				if (registryCache is null) {
					return new ApiBadRequestResponse("Invalid registry?");
				}

				await userComponent.CreateUserOrThrowAsync(
					role: UserModelConst.Role.User,
					signup_type: UserModelConst.SignupType.IdPwd,
					name: registryCache.name,
					email: registryCache.email,
					hashed_password: registryCache.hashed_password
				);

				// Delete cache
				await this.redisComponent.DeleteKeyAsync(cacheKey);

				// Save changes and Commit
				await this.dbContext.SaveChangesAsync();

				return new ApiSuccessResponse();
			}

			return new ApiInternalServerErrorResponse();
		}
	}

	public class UserRegistryCache {
		public string name { get; set; }
		public string email { get; set; }
		public string hashed_password { get; set; }
		public string otp_code { get; set; }
	}
}
