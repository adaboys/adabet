namespace App;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

/// For seeding data via api.
/// Admin+ role is required to run this service.

/// For 3rd-authentication provider (google, facebook,...)
/// https://developers.google.com/identity/one-tap/android/idtoken-auth
/// https://developers.google.com/api-client-library

/// AccessToken and RefreshToken: https://codepedia.info/aspnet-core-jwt-refresh-token-authentication
public class BootCommand : BaseService {
	private readonly ILogger<BootCommand> logger;
	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly UserComponent userComponent;

	public BootCommand(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<BootCommand> logger,
		CardanoNodeRepo cardanoNodeRepo,
		UserComponent userComponent
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.userComponent = userComponent;
	}

	public async Task<ApiResponse> BootProject() {
		// Create root user for this project
		var res1 = await this._CreateRootUserIfNotExist();
		if (res1.failed) {
			return res1;
		}

		// // Insert ABE coin
		// var res2 = await this._InsertProjectCoinIfNotExist();
		// if (res2.failed) {
		// 	return res2;
		// }

		// // Upsert system wallets
		var res3 = await this._UpsertSystemWalletsIfNotExist();
		if (res3.failed) {
			return res3;
		}

		return new ApiSuccessResponse("Done boot project !");
	}

	private async Task<ApiResponse> _CreateRootUserIfNotExist() {
		var user = await this.dbContext.users.FirstOrDefaultAsync(m => m.email == this.appSetting.rootUserTemplate.email);
		if (user != null) {
			this.logger.InfoDk(this, "Skip create root user since existed");
			return new ApiSuccessResponse("Skipped");
		}

		// Begin transaction.
		// Ref: https://docs.microsoft.com/en-us/ef/core/saving/transactions
		using var transaction = await this.dbContext.Database.BeginTransactionAsync();

		try {
			await userComponent.CreateUserOrThrowAsync(
				name: "root",
				signup_type: UserModelConst.SignupType.IdPwd,
				role: UserModelConst.Role.Root,
				email: this.appSetting.rootUserTemplate.email,
				password: this.appSetting.rootUserTemplate.password
			);

			// Save changes and Commit
			await this.dbContext.SaveChangesAsync();
			await transaction.CommitAsync();

			return new ApiSuccessResponse();
		}
		catch (Exception e) {
			// Write log
			this.logger.ErrorDk(this, "Could not create root user, email: {email}, error: {error}",
				this.appSetting.rootUserTemplate.email,
				e.Message
			);

			await transaction.RollbackAsync();

			return new ApiInternalServerErrorResponse("Failed");
		}
	}

	private async Task<ApiResponse> _InsertProjectCoinIfNotExist() {
		var abeCoin = await this.dbContext.cardanoCoins.FirstOrDefaultAsync(m => m.coin_name == MstCardanoCoinModelConst.COIN_NAME_ABE);
		if (abeCoin != null) {
			this.logger.InfoDk(this, "Skip insert ABE coin since existed");
			return new ApiSuccessResponse("Skipped");
		}

		this.dbContext.cardanoCoins.Attach(new() {
			coin_name = MstCardanoCoinModelConst.COIN_NAME_ABE,
			asset_id = this.appSetting.environment == AppSetting.ENV_PRODUCTION ?
				// For mainnet
				"//todo" :
				// For testnet
				"//todo"
		});

		await this.dbContext.SaveChangesAsync();

		return new ApiSuccessResponse();
	}

	private async Task<ApiResponse> _UpsertSystemWalletsIfNotExist() {
		var shouldSaveChanges = false;

		var mainWallet = await this.dbContext.systemWallets.FirstOrDefaultAsync(m => m.type == SystemWalletModelConst.Type.MainForGame);
		if (mainWallet != null) {
			this.logger.InfoDk(this, "Skip upsert ADA wallet since existed");
		}
		else {
			// Create wallet
			var response = await this.cardanoNodeRepo.CreateWalletAsync(SystemWalletModelConst.WalletName_MainForGame);
			if (response.failed) {
				return response;
			}

			this.dbContext.systemWallets.Attach(new() {
				type = SystemWalletModelConst.Type.MainForGame,
				wallet_address = response.data.wallet_address
			});

			shouldSaveChanges = true;
		}

		if (shouldSaveChanges) {
			await this.dbContext.SaveChangesAsync();
		}

		return new ApiSuccessResponse();
	}
}
