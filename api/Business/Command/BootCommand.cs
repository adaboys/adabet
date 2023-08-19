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
		// return await this._BootProject20221129();
		// return await this._Update20230812();

		return new ApiSuccessResponse("Done boot project !");
	}

	private async Task<ApiResponse> _Update20230812() {
		var createSwapWalletResponse = await this.cardanoNodeRepo.CreateWalletAsync(SystemWalletModelConst.WalletName_Swap);
		if (createSwapWalletResponse.failed) {
			return createSwapWalletResponse;
		}

		var abe = await this.dbContext.currencies.FirstAsync(m => m.network == MstCurrencyModelConst.Network.Cardano && m.name == MstCurrencyModelConst.NAME_ABE);
		var gem = await this.dbContext.currencies.FirstAsync(m => m.network == MstCurrencyModelConst.Network.Cardano && m.name == MstCurrencyModelConst.NAME_GEM);

		abe.weight = 250;
		gem.weight = 1000;

		this.dbContext.systemWallets.Attach(new() {
			type = SystemWalletModelConst.Type.Swap,
			wallet_address = createSwapWalletResponse.data.wallet_address
		});

		await this.dbContext.SaveChangesAsync();

		return new ApiSuccessResponse();
	}


	private async Task<ApiResponse> _BootProject20221129() {
		// Create root user for this project
		var res1 = await this._CreateRootUserIfNotExist();
		if (res1.failed) {
			return res1;
		}

		// Insert ABE, GEM coin
		var res2 = await this._InsertProjectCoinIfNotExist();
		if (res2.failed) {
			return res2;
		}

		// Upsert system wallets
		var res3 = await this._UpsertSystemWalletsIfNotExist();
		if (res3.failed) {
			return res3;
		}

		return new ApiSuccessResponse();
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
				signupType: UserModelConst.SignupType.IdPwd,
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
		var foundSomeCoinHasNameAsAbe = await this.dbContext.currencies.AnyAsync(m =>
			m.name == MstCurrencyModelConst.NAME_ABE
		);
		if (!foundSomeCoinHasNameAsAbe) {
			var asset_id = this.appSetting.environment == AppSetting.ENV_PRODUCTION ?
				// For mainnet
				"acc37b7ef2b3bb7855a7ba66ad3a242771092463e184d26f63217208.414245" :
				// For testnet
				"6c6807e4cfd9f1c0fd5322a6467b0e0f132a4b984d98d6d4a97abe5e.74414245"
			;
			this.dbContext.currencies.Attach(new() {
				code = asset_id,
				name = MstCurrencyModelConst.NAME_ABE,
				network = MstCurrencyModelConst.Network.Cardano,
				network_name = MstCurrencyModelConst.NetworkName_Cardano,
				decimals = 6,
			});

			await this.dbContext.SaveChangesAsync();
		}

		var foundSomeCoinHasNameAsGem = await this.dbContext.currencies.AnyAsync(m =>
			m.name == MstCurrencyModelConst.NAME_GEM
		);
		if (!foundSomeCoinHasNameAsGem) {
			var asset_id = this.appSetting.environment == AppSetting.ENV_PRODUCTION ?
				// For mainnet
				"4d0f8074126d4adf93bdcf13c88ea25e25eb002d681e4d0d9e554f12.47454d" :
				// For testnet
				"6c6807e4cfd9f1c0fd5322a6467b0e0f132a4b984d98d6d4a97abe5e.7447454d"
			;
			this.dbContext.currencies.Attach(new() {
				code = asset_id,
				name = MstCurrencyModelConst.NAME_GEM,
				network = MstCurrencyModelConst.Network.Cardano,
				network_name = MstCurrencyModelConst.NetworkName_Cardano,
				decimals = 6,
			});

			await this.dbContext.SaveChangesAsync();
		}

		return new ApiSuccessResponse();
	}

	private async Task<ApiResponse> _UpsertSystemWalletsIfNotExist() {
		var shouldSaveChanges = false;

		var mainWallet = await this.dbContext.systemWallets.FirstOrDefaultAsync(m => m.type == SystemWalletModelConst.Type.Game);
		if (mainWallet != null) {
			this.logger.InfoDk(this, "Skip upsert ADA wallet since existed");
		}
		else {
			// Create wallet
			var response = await this.cardanoNodeRepo.CreateWalletAsync(SystemWalletModelConst.WalletName_Game);
			if (response.failed) {
				return response;
			}

			this.dbContext.systemWallets.Attach(new() {
				type = SystemWalletModelConst.Type.Game,
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
