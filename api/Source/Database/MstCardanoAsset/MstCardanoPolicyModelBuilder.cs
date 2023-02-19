namespace App;

using Microsoft.EntityFrameworkCore;
using Tool.Compet.Core;

public class MstCardanoPolicyModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// Generate default datetime when add.
		modelBuilder.Entity<MstCardanoPolicyModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");
	}
}
