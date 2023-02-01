namespace App;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Tool.Compet.Core;

public class SystemWalletModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<SystemWalletModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}
