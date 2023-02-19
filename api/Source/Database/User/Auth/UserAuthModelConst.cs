namespace App;

public class UserAuthModelConst {
	public enum LoginType {
		IdPwd = 1, // Legacy login with email/password
		Provider = 2, // Login with google, facebook,...
		Token = 3, // Login with tokens from another site (marketplace,...)
		Silent = 4, // Use access_token + refresh_token for login without asking user
		ExternalWallet = 5, // Use external wallet (Nami, Yoroi,...) to login
	}

	public enum ClientType {
		AndroidGame = 1, // From android app
		IosGame = 2, // From ios app
		WebglGame = 3, // From webgl game
		MarketplaceNotGame = 4, // From marketplace site
	}
}
