namespace App;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tool.Compet.Core;

public class UserCommand : BaseService {
	private readonly ILogger<UserCommand> logger;
	private readonly UserComponent userComponent;
	private readonly CardanoNodeRepo cardanoNodeRepo;

	public UserCommand(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UserCommand> logger,
		UserComponent userComponent,
		CardanoNodeRepo cardanoNodeRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.userComponent = userComponent;
		this.cardanoNodeRepo = cardanoNodeRepo;
	}

	public async Task<ApiResponse> UpdateCodesForUsers() {
		var users = await this.dbContext.users.Where(m => m.code == null || m.referral_code == null).ToArrayAsync();

		var updateCount = 0;
		foreach (var user in users) {
			if (user.code is null) {
				user.code = UserComponent.GenerateRandomCodeFromNumber(user.no);
				updateCount++;
			}

			if (user.referral_code is null) {
				user.referral_code = UserComponent.GenerateRandomCodeFromNumber(user.no);
				updateCount++;
			}
		}

		if (updateCount > 0) {
			await this.dbContext.SaveChangesAsync();

			return new ApiSuccessResponse($"Updated {updateCount} users");
		}

		return new ApiSuccessResponse("Nothing to update");
	}

	public async Task<ApiResponse> AddMissingInternalWalletForUsers() {
		// Build map of email-wallet
		var ownedWalletsResponse = await this.cardanoNodeRepo.GetOwnedWalletsAsync();
		if (ownedWalletsResponse.failed) {
			return ownedWalletsResponse;
		}
		var email2wallet = new Dictionary<string, string>();
		foreach (var wallet in ownedWalletsResponse.data.wallets) {
			email2wallet[wallet.client_id] = wallet.wallet_address;
		}

		var users = await this.dbContext.users.ToArrayAsync();
		var addedCount = 0;

		// Add wallets for users
		foreach (var user in users) {
			// Check internal wallet exist for the user
			var existInternalWallet = await this.dbContext.userWallets.AnyAsync(m =>
				m.user_id == user.id &&
				m.wallet_type == UserWalletModelConst.WalletType.Internal
			);
			if (existInternalWallet) {
				continue;
			}

			var wallet = email2wallet.GetValueOrDefault(user.email);
			if (wallet.IsEmptyDk()) {
				throw new AppSystemException($"Wallet of user [{user.id}, {user.email}] must exist");
			}

			this.dbContext.userWallets.Attach(new() {
				user_id = user.id,
				wallet_address = wallet,
				wallet_type = UserWalletModelConst.WalletType.Internal,
				wallet_status = UserWalletModelConst.WalletStatus.Active,
			});

			++addedCount;
		}

		await this.dbContext.SaveChangesAsync();

		return new ApiSuccessResponse($"Added {addedCount} wallets for users");
	}

	public async Task<ApiResponse> SyncUsersToCasino() {
		var query =
			from _user in this.dbContext.users
			join _wallet in this.dbContext.userWallets on _user.id equals _wallet.user_id
			where _wallet.wallet_type == UserWalletModelConst.WalletType.Internal
			select new {
				user = _user,
				wallet_address = _wallet.wallet_address,
				wallet_status = _wallet.wallet_status
			}
		;
		var rows = await query.ToArrayAsync();

		foreach (var row in rows) {
			var user = row.user;

			await this.userComponent.SyncUserToCasino(
				userId: user.id,
				email: user.email,
				role: user.role,
				createdAt: user.created_at,
				playerName: user.player_name,
				internalWalletAddress: row.wallet_address,
				internalWalletAddress_isActive: row.wallet_status is UserWalletModelConst.WalletStatus.Active,
				hashedPassword: user.password
			);
		}

		return new ApiSuccessResponse($"Synced {rows.Length} users to Casino");
	}
}
