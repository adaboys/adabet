namespace App;

public class SportUserBetModelConst {
	public enum BetResult {
		Nothing = 0,
		Won = 1,
		Draw = 2,
		Losed = 3,
	}

	public enum TxStatus {
		RequestSubmitViaApi = 1, // Api will submit to chain
		SubmitSucceed = 3, // The tx was submitted to chain and still in verifying by other nodes
		SubmitFailed = 4, // Could not submit tx to chain (since does not enough balance,...)
		OnchainSucceed = 5, // Finally the tx was published on chain (other nodes got/verified our tx)
	}
}
