namespace App;

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

/// Raw query with interpolated: https://docs.microsoft.com/en-us/ef/core/querying/raw-sql
public class AdminService : BaseService {
	private readonly ILogger<AdminService> logger;
	private readonly CardanoNodeRepo cardanoNodeRepo;

	public AdminService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<AdminService> logger,
		CardanoNodeRepo cardanoNodeRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.cardanoNodeRepo = cardanoNodeRepo;
	}

	public async Task<ApiResponse> GetAdminProfile(Guid userId) {
		var query =
			from _user in this.dbContext.users
			join _wallet in this.dbContext.userWallets on _user.id equals _wallet.user_id

			where _user.id == userId
			where _wallet.wallet_type == UserWalletModelConst.WalletType.Internal

			select new GetAdminProfileResponse.Data {
				name = _user.name,
				player_name = _user.player_name,
				email = _user.email,
				code = _user.code,
				telno = _user.telno,
				wallet_address = _wallet.wallet_address,
				has_password = _user.password != null,
				avatar = _user.avatar_relative_path == null ? null : $"{this.appSetting.s3.baseUrl}/{_user.avatar_relative_path}",
			}
		;
		var data = await query.FirstOrDefaultAsync();
		if (data is null) {
			return new ApiBadRequestResponse("Not found user/wallet");
		}

		return new GetAdminProfileResponse {
			data = data
		};
	}
}
