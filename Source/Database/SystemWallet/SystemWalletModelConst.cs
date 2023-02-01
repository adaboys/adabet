namespace App;

public class SystemWalletModelConst {
	/// Each wallet name is mapped with a type.
	/// They are client_id when create wallet at cnode.
	public const string WalletName_MainForGame = "main_for_game";

	public enum Type {
		MainForGame = 1, // We use this to submit coin to winners.
	}
}
