namespace App;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

public class UserWalletModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<UserWalletModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}
