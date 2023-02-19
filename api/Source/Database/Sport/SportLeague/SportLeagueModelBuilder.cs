namespace App;

using Microsoft.EntityFrameworkCore;

public class SportLeagueModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<SportLeagueModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}
