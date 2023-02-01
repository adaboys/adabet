namespace App;

using Microsoft.EntityFrameworkCore;

public class SportTeamModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<SportTeamModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}
