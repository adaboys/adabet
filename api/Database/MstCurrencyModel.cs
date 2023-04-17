namespace App;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Tool.Compet.Core;

/// Currency for payment of order.
[Table(DbConst.table_mst_currency)]
[Index(nameof(code), nameof(network), IsUnique = true)]
[Index(nameof(name))]
public class MstCurrencyModel : AutoGenerateUpdateTime {
	/// PK
	[Key]
	[Column("id")]
	public int id { get; set; }

	/// Unique code represent for the currency.
	/// For cardano, this is `lovelace` or asset_id (policy_id.asset_name_in_hex).
	/// For real money, this is id-name as: VND, USD, YEN,...
	[Required]
	[Column("code", TypeName = "varchar(255)"), MaxLength(255)]
	public string code { get; set; }

	/// Human readable name represent for the currency, for eg,. ADA, ABE, GEM, VND, USD,...
	/// It is useful for display currency name at user side.
	[Required]
	[Column("name", TypeName = "varchar(64)"), MaxLength(64)]
	public string name { get; set; }

	/// In general, each currency is used in a network, for eg,. ADA, ISKY are used in Cardano network.
	/// To get currency info, we need contact to target network.
	[Required]
	[Column("network", TypeName = "tinyint")]
	public MstCurrencyModelConst.Network network { get; set; }

	/// Human readable name for the network, for eg,. Cardano, Bitcoin, Ethereum,...
	/// It is useful for display network name at user side.
	[Required]
	[Column("network_name", TypeName = "varchar(64)"), MaxLength(64)]
	public string network_name { get; set; }

	/// For blockchain, in general, each coin has own decimal value.
	/// For eg,. ADA has 6 decimals (for eg,. 38.321456).
	/// We store decimal value to calculate actual token quantity.
	[Column("decimals")]
	public int decimals { get; set; }

	[Required]
	[Column("created_at")]
	public DateTime created_at { get; set; }

	[Column("updated_at")]
	public DateTime? updated_at { get; set; }
}

public class MstCurrencyModelBuilder {
	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// [Modelizing]
		// Generate default datetime when add.
		modelBuilder.Entity<MstCurrencyModel>().Property(model => model.created_at).HasDefaultValueSql("getutcdate()");

		// [Seeding]
		modelBuilder.Entity<MstCurrencyModel>().ToTable(DbConst.table_mst_currency).HasData(
			new MstCurrencyModel().AlsoDk(model => {
				model.id = 1;
				model.code = MstCurrencyModelConst.CODE_ADA;
				model.name = MstCurrencyModelConst.NAME_ADA;
				model.network = MstCurrencyModelConst.Network.Cardano;
				model.network_name = MstCurrencyModelConst.NetworkName_Cardano;
				model.decimals = 6;
			})
		);
	}
}

public class MstCurrencyModelConst {
	public enum Network {
		Cardano = 1,
		// Bitcoin = 2,
		// Eth = 3,
	}

	/// For field `code`
	public const string CODE_ADA = "lovelace";

	/// For field `name`
	public const string NAME_ADA = "ADA";
	public const string NAME_ABE = "ABE";
	public const string NAME_GEM = "GEM";

	/// For field `network_name`
	public const string NetworkName_Cardano = "Cardano";
}
