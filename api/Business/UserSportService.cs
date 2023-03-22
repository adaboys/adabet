namespace App;

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tool.Compet.Core;
using Tool.Compet.Json;

public class UserSportService : BaseService {
	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly SystemWalletDao systemWalletDao;

	public UserSportService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		CardanoNodeRepo cardanoNodeRepo,
		SystemWalletDao systemWalletDao
	) : base(dbContext, snapshot) {
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.systemWalletDao = systemWalletDao;
	}

	public async Task<ApiResponse> PlaceBet(Guid userId, int sport_id, Sport_PlaceBetRequestBody reqBody) {
		// Get target matches
		var targetMatchIds = reqBody.bets.Select(m => m.match_id).ToArray();
		var queryMatches =
			from _match in this.dbContext.sportMatches
			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id

			where _league.sport_id == sport_id
			where targetMatchIds.Contains(_match.id)
			where SportMatchModelConst.BettableMatchStatusList.Contains(_match.status)

			orderby _match.updated_at descending

			select new {
				league = _league.name,
				match_id = _match.id,
				markets = _match.markets,
			}
		;
		var matchResultItems = await queryMatches.AsNoTracking().ToDictionaryAsync(m => m.match_id);
		if (matchResultItems.Count != reqBody.bets.Length) {
			return new ApiBadRequestResponse("The request contains invalid match status") { code = ErrCode.invalid_match };
		}

		// Validate req-bets (market + odds)
		// Target on each match -> target on each market -> target on each odd.
		var req_totalBetAdaAmount = 0m;
		foreach (var req_bet in reqBody.bets) {
			var match = matchResultItems.GetValueOrDefault(req_bet.match_id);
			if (match is null) {
				return new ApiBadRequestResponse($"Not found match {req_bet.match_id}") { code = ErrCode.not_found_match };
			}
			var markets = DkJsons.ToObj<List<Market>>(match.markets) ?? new();
			var name2market = markets.ToDictionary(m => m.name);
			if (name2market.Count == 0) {
				return new ApiBadRequestResponse($"Market of the match is unavailable for now") { code = ErrCode.market_unavailable };
			}

			// Target on each market.
			// Each req-market must found in db-markets of the match
			foreach (var req_market in req_bet.markets) {
				var market = name2market.GetValueOrDefault(req_market.name);
				if (market is null) {
					return new ApiBadRequestResponse($"Not found market {req_market.name}") { code = ErrCode.not_found_market };
				}

				// Target on each odd in the market.
				// Each req-odd must found in db-odds of the match's market
				var name2odd = market.odds.ToDictionary(m => m.name);
				foreach (var req_odd in req_market.odds) {
					var odd = name2odd.GetValueOrDefault(req_odd.name);
					if (odd is null) {
						return new ApiBadRequestResponse($"Not found odd {req_odd.name}") { code = ErrCode.not_found_odd };
					}
					if (odd.value <= AppConst.ODD_VALUE_MIN_EXCLUSIVE) {
						return new ApiBadRequestResponse($"Current odd {odd.name} is unavailable for betting");
					}
					if (odd.value != req_odd.value) {
						return new ApiBadRequestResponse("Odd changed, please reload to try with new bet") { code = ErrCode.odd_changed };
					}

					req_totalBetAdaAmount += req_odd.ada_amount;
				}
			}
		}

		// Validate user balance (enough or not)
		var userWalletAddress = await this.dbContext.userWallets
			.Where(m =>
				m.user_id == userId &&
				m.wallet_type == UserWalletModelConst.WalletType.Internal &&
				m.wallet_status == UserWalletModelConst.WalletStatus.Active &&
				m.deleted_at == null
			)
			.Select(m => m.wallet_address)
			.FirstOrDefaultAsync()
		;
		if (userWalletAddress is null) {
			return new ApiBadRequestResponse("User wallet is not ready to use");
		}
		var balanceResponse = await this.cardanoNodeRepo.GetMergedAssetsAsync(userWalletAddress);
		if (balanceResponse.failed) {
			return balanceResponse;
		}
		var holdAdaAmount = CardanoHelper.CalcTotalAdaFromAssets(balanceResponse.data.assets);
		if (holdAdaAmount - AppConst.MIN_FEE_IN_ADA < req_totalBetAdaAmount) {
			return new ApiBadRequestResponse("User's balance does not enough to bet") { code = ErrCode.balance_not_enough };
		}

		// Validation OK !
		// Register user bet and submit to blockchain

		// Get default internal wallet as reward address
		var reward_receiver_address = reqBody.reward_address;
		if (reward_receiver_address is null) {
			reward_receiver_address = userWalletAddress;
		}
		// Validate requested reward address
		else {
			if (!CardanoHelper.IsAddressMatchWithEnv(this.appSetting, reward_receiver_address)) {
				return new ApiBadRequestResponse("Address does not match with current env") { code = ErrCode.invalid_address };
			}
			var validationResponse = await this.cardanoNodeRepo.ValidateAddressAsync(reward_receiver_address);
			if (validationResponse.failed) {
				return validationResponse;
			}
		}

		// Save user bets
		var userBets = new List<SportUserBetModel>();
		foreach (var req_bet in reqBody.bets) {
			foreach (var req_market in req_bet.markets) {
				foreach (var req_odd in req_market.odds) {
					userBets.Add(new() {
						user_id = userId,
						sport_match_id = req_bet.match_id,

						submit_tx_status = SportUserBetModelConst.TxStatus.RequestSubmitViaApi,

						bet_market_name = req_market.name,
						bet_odd_name = req_odd.name,
						bet_odd_value = req_odd.value,

						bet_ada_amount = req_odd.ada_amount,

						reward_address = reward_receiver_address
					});
				}
			}
		}
		this.dbContext.sportUserBets.AttachRange(userBets);
		await this.dbContext.SaveChangesAsync();

		// Increment bet count
		await this.dbContext.sportMatches.ExecuteUpdateAsync(s => s.SetProperty(m => m.user_bet_count, m => m.user_bet_count + 1));

		// Try to submit the bet to chain.
		// If it failed, then cronjob will try submit later for us.
		var submitResponse = await this._TrySubmitToChainAsync(userBets);

		return new ApiSuccessResponse($"Submitted to chain: {submitResponse.succeed}");
	}

	private async Task<ApiResponse> _TrySubmitToChainAsync(List<SportUserBetModel> userBets) {
		var txFailed = true;
		string? txId = null;
		string? txResultMessage = null;

		try {
			var userBetIds = userBets.Select(m => m.id).ToArray();
			var queryBets =
				from _ubet in this.dbContext.sportUserBets
				join _match in this.dbContext.sportMatches on _ubet.sport_match_id equals _match.id
				join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
				join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
				join _wallet in this.dbContext.userWallets on _ubet.user_id equals _wallet.user_id

				where userBetIds.Contains(_ubet.id)
				where _ubet.submit_tx_status == SportUserBetModelConst.TxStatus.RequestSubmitViaApi
				where _wallet.wallet_type == UserWalletModelConst.WalletType.Internal && _wallet.wallet_status == UserWalletModelConst.WalletStatus.Active

				select new {
					_wallet.wallet_address,

					home_team_name = _team1.name,
					away_team_name = _team2.name,

					match_start_at = _match.start_at,

					bet_market_name = _ubet.bet_market_name,
					bet_odd_name = _ubet.bet_odd_name,
					bet_odd_value = _ubet.bet_odd_value,
					bet_ada_amount = _ubet.bet_ada_amount,
				}
			;
			var pendingBets = await queryBets.ToArrayAsync();
			if (pendingBets.Length != userBetIds.Length) {
				throw new AppSystemException(txResultMessage = $"Query result count ({pendingBets.Length}) does not match with the bets ({userBets.Count})");
			}

			// We use CIP-20 (datum label as 674) to attach user's bet as message
			// Ref: https://cips.cardano.org/cips/cip20
			var bet_memos = new List<string>();
			var sysWalletAddr = await this.systemWalletDao.GetSystem_MainForGame_AddressAsync();
			var totalBetAdaAmount = 0m;
			string? userInternalWalletAddress = null;

			foreach (var item in pendingBets) {
				bet_memos.Add($"t1: {item.home_team_name}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"t2: {item.away_team_name}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"start: {item.match_start_at.FormatDk()}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"market: {item.bet_market_name}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"odn: {item.bet_odd_name}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"odv: {item.bet_odd_value}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"bet: {item.bet_ada_amount} ADA".TruncateAsMetadataEntryDk());

				totalBetAdaAmount += item.bet_ada_amount;
				if (userInternalWalletAddress is null) {
					userInternalWalletAddress = item.wallet_address;
				}
			}

			var betUserWalletAddr = userInternalWalletAddress!;
			var betMetadata = new BetMetadata {
				cip_674 = new() {
					msg_list = bet_memos.ToArray()
				}
			};
			var cardanoRequest = new CardanoNode_SendAssetsRequestBody {
				sender_address = betUserWalletAddr,
				receiver_address = sysWalletAddr,
				fee_payer_address = betUserWalletAddr,
				discount_fee_from_assets = false,

				metadata = betMetadata,

				assets = new CardanoNode_AssetInfo[] {
					new() {
						asset_id = MstCardanoCoinModelConst.ASSET_ID_ADA,
						quantity = $"{(totalBetAdaAmount * AppConst.ADA_COIN2TOKEN):0}"
					}
				}
			};

			// Send to chain and Update status
			var cardanoResponse = await this.cardanoNodeRepo.SendAssetsAsync(cardanoRequest);

			txFailed = cardanoResponse.failed;
			txId = cardanoResponse.data?.tx_id;
			txResultMessage = cardanoResponse.message;

			return cardanoResponse;
		}
		catch (Exception e) {
			txResultMessage = e.Message;

			return new ApiInternalServerErrorResponse("Could not submit bets to chain");
		}
		// Update tx-status at finally block
		finally {
			foreach (var ubet in userBets) {
				ubet.submit_tx_status = txFailed ? SportUserBetModelConst.TxStatus.SubmitFailed : SportUserBetModelConst.TxStatus.SubmitSucceed;
				ubet.submit_tx_id = txId;
				ubet.submit_tx_result_message = txResultMessage.TruncateForShortLengthDk();
			}
			await this.dbContext.SaveChangesAsync();
		}
	}
}
