namespace App;

public class SportWinnerBetRewardTxModelConst {
	public enum TxStatus {
		RequestSubmitToChain = 1, // Requesting send reward
		SubmitSucceed = 2, // Submit succeed
		SubmitFailed = 3, // Submit failed
	}
}
