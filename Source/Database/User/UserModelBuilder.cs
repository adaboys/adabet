namespace App;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

public class UserModelBuilder {
	/// Seeding data:
	/// Note that: we are using `ModelBuilder` for seeding data.
	/// We must manually generate PK for each model since this approach uses migration scripts
	/// to calculate next script, so it does not interact with the database.
	/// Data seeding: https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding
	/// Data seeding: https://github.com/dotnet/EntityFramework.Docs/blob/main/entity-framework/core/modeling/data-seeding.md
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// - PK:
		// Setup default unique identifier when create
		// By default, EF will use `newsequentialid()` PK for better performance.
		// We should not set default value since it will be auto handled by framework.
		// - Composite keys:
		// You can also configure multiple properties to be the key of an entity - this is known as a composite key.
		// Composite keys can only be configured using the Fluent API; conventions will never set up a composite key,
		// and you can not use Data Annotations to configure one.
		// Ref: https://learn.microsoft.com/en-us/ef/core/modeling/keys?tabs=data-annotations
		// modelBuilder.Entity<UserModel>().HasKey(c => new { c.id, c.code });

		// Generate default datetime when add.
		// Note that: we should NOT use `ValueGeneratedOnAdd()` since it is no effect for DateTime type on MS SQL.
		// Let use `HasDefaultValueSql()` instead.
		// Ref: https://docs.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=fluent-api#datetime-value-generation
		modelBuilder.Entity<UserModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");

		// Ref: https://docs.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=fluent-api
		// Unlike with default values or computed columns, we are not specifying how the values are to be generated;
		// that depends on the database provider being used. Database providers may automatically set up
		// value generation for some property types, but others may require you to manually set up how the value is generated.
		// - For example, on SQL Server, when a GUID property is configured as value generated on add, the provider
		// automatically performs value generation client-side, using an algorithm to generate optimal
		// sequential GUID values. However, specifying ValueGeneratedOnAdd on a DateTime property will
		// have no effect (see the section below for DateTime value generation).
		// - Similarly, byte[] properties that are configured as generated on add or update and marked as concurrency
		// tokens are set up with the rowversion data type, so that values are automatically generated in the database.
		// However, specifying ValueGeneratedOnAdd has no effect.
		// modelBuilder.Entity<UserModel>().Property(model => model.updated_at).HasDefaultValueSql("getdate()").ValueGeneratedOnAddOrUpdate();

		// Seed
		// modelBuilder.Entity<UserModel>().ToTable(DbConst.table_user).HasData(new UserModel() {
		// 	id = Guid.NewGuid(),
		// 	code = "darkcompet",
		// 	email = "darkcompet@gmail.com"
		// });
	}
}
