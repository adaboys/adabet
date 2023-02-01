namespace App;

using Microsoft.EntityFrameworkCore;

public class SportUserBetModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<SportUserBetModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}
