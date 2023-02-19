namespace App;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

public class UserAuthModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<UserAuthModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}
