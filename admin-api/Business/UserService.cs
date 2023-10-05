namespace App;

using System.Text;
using System.Transactions;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RedLockNet;
using Tool.Compet.Core;
using Tool.Compet.EntityFrameworkCore;
using Tool.Compet.Json;

/// Raw query with interpolated: https://docs.microsoft.com/en-us/ef/core/querying/raw-sql
public class UserService : BaseService {
	private readonly ILogger<UserService> logger;
	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly UserDao userDao;
	private readonly MailComponent mailService;
	private readonly UserComponent userComponent;
	private readonly RedisComponent redisComponent;
	private readonly IDistributedLockFactory lockFactory;
	private readonly IAmazonS3 awsS3Client;

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

		var s3 = this.appSetting.s3;
		this.awsS3Client = new AmazonS3Client(
			awsAccessKeyId: s3.accessKeyId,
			awsSecretAccessKey: s3.secretAccessKey,
			region: RegionEndpoint.GetBySystemName(s3.region)
		);
	}

	public class UserRegistryCache {
		public string name { get; set; }
		public string email { get; set; }
		public string hashed_password { get; set; }
		public string otp_code { get; set; }
	}

	public async Task<ApiResponse> GetUserProfile(Guid userId) {
		var query =
			from _user in this.dbContext.users
			join _wallet in this.dbContext.userWallets on _user.id equals _wallet.user_id

			where _user.id == userId && _wallet.wallet_type == UserWalletModelConst.WalletType.Internal

			select new {
				name = _user.name,
				player_name = _user.player_name,
				email = _user.email,
				code = _user.code,
				telno = _user.telno,
				referral_code = _user.referral_code,
				wallet_address = _wallet.wallet_address,
				has_password = _user.password != null,
				avatar_relative_path = _user.avatar_relative_path,
			}
		;
		var result = await query.FirstOrDefaultAsync();
		if (result is null) {
			return new ApiBadRequestResponse("Not found user/wallet");
		}

		return new GetUserProfileResponse {
			data = new() {
				name = result.name,
				player_name = result.player_name,
				email = result.email,
				code = result.code,
				telno = result.telno,
				referral_code = result.referral_code,
				wallet_address = result.wallet_address,
				has_password = result.has_password,
				avatar = result.avatar_relative_path is null ? null : $"{this.appSetting.s3.baseUrl}/{result.avatar_relative_path}",
				vip_level = 1,
				cur_vip_point = 100,
				next_vip_point = 200,
			}
		};
	}

	public async Task<ApiResponse> ChangePassword(Guid userId, ChangePasswordRequestBody requestBody, string? ipAddress, string? userAgent) {
		var user = await this.userDao.FindValidUserViaIdAsync(userId);
		if (user is null) {
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
		if (user is null) {
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

	public async Task<ApiResponse> UpdateAvatar(Guid userId, UpdateUserAvatarRequestBody requestBody) {
		var user = await this.dbContext.users.FirstAsync(m => m.id == userId);

		var file = requestBody.avatar;
		var fileExt = file.FileName.SubstringDk(file.FileName.LastIndexOf('.'));
		if (fileExt.Trim().Length == 0) {
			return new ApiBadRequestResponse("Cannot get file ext");
		}

		try {
			using (var memoryStream = new MemoryStream()) {
				file.CopyTo(memoryStream);

				var avatarRelativePath = S3Paths.ForUserAvatar(Guid.NewGuid().ToStringDk() + fileExt);
				var uploadRequest = new TransferUtilityUploadRequest {
					InputStream = memoryStream,
					Key = avatarRelativePath,
					BucketName = this.appSetting.s3.bucketName,
					ContentType = file.ContentType
				};

				var fileTransferUtility = new TransferUtility(this.awsS3Client);

				await fileTransferUtility.UploadAsync(uploadRequest);

				// Update avatar path
				user.avatar_relative_path = avatarRelativePath;
				await this.dbContext.SaveChangesAsync();

				return new UpdateUserAvatarResponse {
					data = new() {
						avatar = $"{this.appSetting.s3.baseUrl}/{user.avatar_relative_path}"
					}
				};
			}
		}
		catch (Exception e) {
			return new ApiInternalServerErrorResponse(e.Message);
		}
	}

	public async Task<ApiResponse> SearchUsers(
		int pagePos, int pageSize, // Pagination
		string? sortType, // Sort on each field
		string? keyword // Search all
	) {
		var query =
			from _user in this.dbContext.users

			join _wallet in this.dbContext.userWallets on _user.id equals _wallet.user_id

			where _wallet.wallet_type == UserWalletModelConst.WalletType.Internal

			select new GetUserListResponse.User {
				id = _user.id,
				name = _user.name,
				player = _user.player_name,
				role = (int)_user.role,
				status = (int)_user.status,
				login_locked_until = _user.login_locked_until,
				created_at = _user.created_at,
				last_login = _user.last_login_at,

				wallet_address = _wallet.wallet_address,
				wallet_balance = _wallet.balance == null ? null : DkJsons.ToObj<List<GetUserListResponse.UserWalletBalance>>(_wallet.balance),
			}
		;

		switch (sortType) {
			case "created_at_asc": {
				query = query.OrderBy(m => m.created_at);
				break;
			}
			case "created_at_desc": {
				query = query.OrderByDescending(m => m.created_at);
				break;
			}
			case "name_asc": {
				query = query.OrderBy(m => m.name);
				break;
			}
			case "name_desc": {
				query = query.OrderByDescending(m => m.name);
				break;
			}
		}

		if (keyword != null) {
			query = query.Where(m =>
				(m.name != null && m.name.Contains(keyword)) ||
				m.player.Contains(keyword) ||
				m.wallet_address.Contains(keyword)
			);
		}

		var pagedResult = await query.AsNoTracking().PaginateDk(pagePos, pageSize);
		var result = pagedResult.items;

		// Response coin name
		var coins = await this.dbContext.currencies.AsNoTracking().Select(m => new { m.id, m.name }).ToDictionaryAsync(m => m.id);
		foreach (var item in result) {
			if (item.wallet_balance != null) {
				foreach (var balance in item.wallet_balance) {
					balance.coin_name = coins[balance.coin_id].name;
				}
			}
		}

		return new GetUserListResponse {
			data = new() {
				page_pos = pagedResult.pagePos,
				page_count = pagedResult.pageCount,
				total_item_count = pagedResult.totalItemCount,
				users = result
			}
		};
	}

	public async Task<ApiResponse> UpdateUser(Guid user_id, UpdateUserRequestBody reqBody) {
		var user = await this.dbContext.users.FirstAsync(m => m.id == user_id);

		var msg = new StringBuilder();

		if (reqBody.block_login != null) {
			var blockLogin = (bool)reqBody.block_login;
			if (!blockLogin) {
				user.login_failed_count = 0;
			}
			user.login_locked_until = blockLogin ? DateTime.UtcNow.AddYears(100) : null;
			msg.Append($"[Unlocked login: {blockLogin}] ");
		}

		if (reqBody.block_user != null) {
			var blockUser = (bool)reqBody.block_user;
			user.status = blockUser ? UserModelConst.Status.Blocked : UserModelConst.Status.Normal;
			msg.Append($"[Unlocked user: {blockUser}] ");
		}

		if (msg.Length > 0) {
			await this.dbContext.SaveChangesAsync();
		}

		return new ApiSuccessResponse(msg.ToString());
	}
}
