namespace App;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public class UserCommand : BaseService {
	private readonly ILogger<UserCommand> logger;
	private readonly UserComponent userComponent;
	private readonly CasinoRepo casinoRepo;

	public UserCommand(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UserCommand> logger,
		UserComponent userComponent,
		CasinoRepo casinoRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.userComponent = userComponent;
		this.casinoRepo = casinoRepo;
	}

	public async Task<ApiResponse> UpdateCodesForUsers() {
		var users = await this.dbContext.users.Where(m => m.code == null || m.referral_code == null).ToArrayAsync();

		var updateCount = 0;
		foreach (var user in users) {
			if (user.code is null) {
				user.code = UserComponent.CalcCodeFromSeed(user.no);
				updateCount++;
			}

			if (user.referral_code is null) {
				user.referral_code = UserComponent.CalcCodeFromSeed(user.no);
				updateCount++;
			}
		}

		if (updateCount > 0) {
			await this.dbContext.SaveChangesAsync();

			return new ApiSuccessResponse($"Updated {updateCount} users");
		}

		return new ApiSuccessResponse("Nothing to update");
	}

	public async Task<ApiResponse> SyncUsersToCasino() {
		var query =
			from _user in this.dbContext.users

			join _wallet in this.dbContext.userWallets on _user.id equals _wallet.user_id

			where _wallet.wallet_type == UserWalletModelConst.WalletType.Internal

			select new {
				user_id = _user.id,
				user_name = _user.player_name,
				email = _user.email,
				password = _user.password,
				wallet = _wallet.wallet_address
			}
		;
		var users = await query.AsNoTracking().ToArrayAsync();
		Casino_RegisterUserResponse? response = null;

		foreach (var user in users) {
			response = await this.casinoRepo.RegisterUser(
				user_id: user.user_id,
				name: user.user_name,
				email: user.email,
				password: null,
				wallet_address: user.wallet
			);
			if (response is null || response.failed) {
				this.logger.WarningDk(this, "Failed res: {@data}", response);
			}
		}

		return response ?? new Casino_RegisterUserResponse() { status = 500, message = "Could not complete sync" };
	}
}
