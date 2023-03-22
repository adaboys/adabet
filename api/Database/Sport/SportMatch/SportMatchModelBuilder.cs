namespace App;

using Microsoft.EntityFrameworkCore;

public class SportMatchModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<SportMatchModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");

		// // Ref: https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew
		// modelBuilder.Entity<SportMatchModel>().OwnsMany(m => m.markets, ownedNavigationBuilder => {
		// 	ownedNavigationBuilder.ToJson();
		// 	ownedNavigationBuilder.OwnsMany(m1 => m1.odds);
		// });
	}
}
