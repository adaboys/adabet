namespace App;

using Microsoft.EntityFrameworkCore;

public class SportWinnerBetRewardTxModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<SportWinnerBetRewardTxModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}
