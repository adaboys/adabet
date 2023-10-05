namespace App;

public class BetsapiHelper {
	/// As betsapi's logic, a returned match_id in betsapi maybe does not equals to our requested `ref_betsapi_match_id`.
	/// When it happens, it means that `ref_betsapi_match_id` was changed to other id of api-match.
	/// To solve it, we just try to call api event/view and update `ref_betsapi_match_id` for the sysMatch.
	public static async Task RemapMatchIdAsync(SportMatchModel sysMatch, BetsapiRepo betsapiRepo, ILogger logger, object caller) {
		var apiResponse = await betsapiRepo.FetchMatchDetail<BetsapiData_MatchDetail_Base>(sysMatch.ref_betsapi_match_id);
		if (apiResponse is null || apiResponse.failed) {
			logger.ErrorDk(caller, "Could not remap for sys-match {1} ({2}). Api response: {@data}", sysMatch.id, sysMatch.ref_betsapi_match_id, apiResponse);
			return;
		}

		// Update ref_betsapi_match_id and lock betting + prediction to avoid take action on it.
		if (apiResponse.results.Count > 0) {
			logger.InfoDk(caller, "Remapped for sys-match {1} ({2} -> {3})", sysMatch.id, sysMatch.ref_betsapi_match_id, apiResponse.results[0].id);

			sysMatch.ref_betsapi_match_id = apiResponse.results[0].id;
			sysMatch.lock_mode = SportMatchModelConst.LockMode.LockBetting;
		}
		else {
			logger.WarningDk(caller, "Fetched but no id to remap for sys-match {1} ({2})", sysMatch.id, sysMatch.ref_betsapi_match_id);
		}
	}
}
