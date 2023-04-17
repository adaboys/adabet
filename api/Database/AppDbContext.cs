namespace App;

using System.Transactions;
using Microsoft.EntityFrameworkCore;

/// Database management for the app.
public class AppDbContext : DbContext {
	/// We need this constructor for configuration via `appsetting.json`
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	/// Declare all models for LinQ query translation

	/// [System]
	public DbSet<SystemWalletModel> systemWallets { get; set; }

	/// [Master]
	public DbSet<MstAppModel> apps { get; set; }
	public DbSet<MstCountryModel> countries { get; set; }
	public DbSet<MstCurrencyModel> currencies { get; set; }
	public DbSet<MstExchangeRateModel> exchangeRates { get; set; }

	/// [User]
	public DbSet<UserModel> users { get; set; }
	public DbSet<UserAuthModel> userAuths { get; set; }
	public DbSet<UserWalletModel> userWallets { get; set; }

	// [Sport]
	public DbSet<MstSportModel> sports { get; set; }
	public DbSet<SportLeagueModel> sportLeagues { get; set; }
	public DbSet<SportTeamModel> sportTeams { get; set; }
	public DbSet<SportMatchModel> sportMatches { get; set; }
	public DbSet<SportUserBetModel> sportUserBets { get; set; }
	public DbSet<SportWinnerBetRewardTxModel> sportWinnerBetRewardTxs { get; set; }

	public DbSet<SportPredictUserModel> sportPredictUsers { get; set; }

	/// Construct model + Seeding data
	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		// System
		SystemWalletModelBuilder.OnModelCreating(modelBuilder);

		// Master
		MstAppModelBuilder.OnModelCreating(modelBuilder);
		MstCurrencyModelBuilder.OnModelCreating(modelBuilder);
		MstCountryModelBuilder.OnModelCreating(modelBuilder);
		MstExchangeRateModelBuilder.OnModelCreating(modelBuilder);

		// User
		UserModelBuilder.OnModelCreating(modelBuilder);
		UserAuthModelBuilder.OnModelCreating(modelBuilder);
		UserWalletModelBuilder.OnModelCreating(modelBuilder);


		// Sport
		MstSportModelBuilder.OnModelCreating(modelBuilder);
		SportLeagueModelBuilder.OnModelCreating(modelBuilder);
		SportTeamModelBuilder.OnModelCreating(modelBuilder);
		SportMatchModelBuilder.OnModelCreating(modelBuilder);
		SportUserBetModelBuilder.OnModelCreating(modelBuilder);
		SportWinnerBetRewardTxModelBuilder.OnModelCreating(modelBuilder);

		SportPredictUserModelBuilder.OnModelCreating(modelBuilder);
	}

	/// @Override
	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
		var now = DateTime.UtcNow;

		var modifiedEntries = this.ChangeTracker.Entries()
			.Where(x => x.State == EntityState.Modified)
			.Select(x => x.Entity)
		;
		foreach (var modifiedEntry in modifiedEntries) {
			var entity = modifiedEntry as AutoGenerateUpdateTime;
			if (entity != null) {
				entity.updated_at = now;
			}
		}

		return base.SaveChangesAsync(cancellationToken);
	}

	/// Utility for transaction scope with auto-dispose resource when completed.
	/// By default, we start transaction flow that across thread continuations enabled.
	public static void NewTransactionScope(Action<TransactionScope> callback) {
		using (var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
			callback(txScope);
		};
	}
}
