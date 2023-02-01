namespace App;

public class UserWalletModelConst {
	public enum WalletType {
		Internal = 1, // our system wallet
		External = 2, // external system wallet
	}

	public enum WalletStatus {
		Pending = 1, // sometime we need review the wallet before use
		Active = 2, // can use normally
		Inactive = 3, // invalid for use, for eg,. user has requested lock the wallet for a while.
	}
}
