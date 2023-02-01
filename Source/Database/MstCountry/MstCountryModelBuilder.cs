namespace App;

using Microsoft.EntityFrameworkCore;

public class MstCountryModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<MstCountryModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}
