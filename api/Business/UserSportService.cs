namespace App;

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tool.Compet.Core;
using Tool.Compet.EntityFrameworkCore;
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
			where _match.lock_mode == SportMatchModelConst.LockMode.Nothing

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
		var req_totalBetCurrencyAmount = 0m;
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

					req_totalBetCurrencyAmount += req_odd.bet_amount;
				}
			}
		}

		// Since user will bet with internal wallet,
		// so first, we soft validate user balance.
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

		var betWithCoin = await this.dbContext.currencies.FirstOrDefaultAsync(m => m.id == reqBody.bet_currency);
		if (betWithCoin is null) {
			return new ApiBadRequestResponse("Invalid bet currency");
		}
		var balanceResponse = await this.cardanoNodeRepo.GetMergedAssetsAsync(userWalletAddress);
		if (balanceResponse.failed) {
			return balanceResponse;
		}
		var holdCoinAmount = CardanoHelper.CalcTotalCoinFromAssets(betWithCoin, balanceResponse.data.assets);
		if (holdCoinAmount < req_totalBetCurrencyAmount) {
			return new ApiBadRequestResponse("User's balance does not enough to bet") { code = ErrCode.balance_not_enough };
		}

		// Validate requested reward address
		if (reqBody.reward_address != null) {
			if (!CardanoHelper.IsAddressMatchWithEnv(this.appSetting, reqBody.reward_address)) {
				return new ApiBadRequestResponse("Address does not match with current env") { code = ErrCode.invalid_address };
			}
			var validationResponse = await this.cardanoNodeRepo.ValidateAddressAsync(reqBody.reward_address);
			if (validationResponse.failed) {
				return validationResponse;
			}
		}

		var reward_receiver_address = reqBody.reward_address ?? userWalletAddress;

		// Register user bet and submit to blockchain
		var userBets = new List<SportUserBetModel>();
		foreach (var req_bet in reqBody.bets) {
			foreach (var req_market in req_bet.markets) {
				foreach (var req_odd in req_market.odds) {
					userBets.Add(new() {
						user_id = userId,
						sport_match_id = req_bet.match_id,

						submit_tx_status = SportUserBetModelConst.TxStatus.Draft,

						bet_market_name = req_market.name,
						bet_odd_name = req_odd.name,
						bet_odd_value = req_odd.value,

						bet_currency_id = betWithCoin.id,
						bet_currency_amount = req_odd.bet_amount,

						reward_address = reward_receiver_address
					});
				}
			}
		}
		this.dbContext.sportUserBets.AttachRange(userBets);
		await this.dbContext.SaveChangesAsync();

		// Increment bet count
		await this.dbContext.sportMatches.ExecuteUpdateAsync(s => s.SetProperty(m => m.user_bet_count, m => m.user_bet_count + 1));

		// Submit the bet to chain. And update user bets.
		return await this._SubmitBetsToChainAndUpdateTxStatusAsync(userBets, betWithCoin);
	}

	private async Task<ApiResponse> _SubmitBetsToChainAndUpdateTxStatusAsync(List<SportUserBetModel> userBets, MstCurrencyModel betWithCoin) {
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
				join _coin in this.dbContext.currencies on _ubet.bet_currency_id equals _coin.id

				where userBetIds.Contains(_ubet.id)
				where _wallet.wallet_type == UserWalletModelConst.WalletType.Internal
				where _wallet.wallet_status == UserWalletModelConst.WalletStatus.Active

				select new {
					_wallet.wallet_address,

					home_team_name = _team1.name,
					away_team_name = _team2.name,

					match_start_at = _match.start_at,

					bet_market_name = _ubet.bet_market_name,
					bet_odd_name = _ubet.bet_odd_name,
					bet_odd_value = _ubet.bet_odd_value,

					bet_coin_name = _coin.name,
					bet_coin_amount = _ubet.bet_currency_amount,
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
			var totalBetCoinAmount = 0m;
			string? userInternalWalletAddress = null;

			foreach (var item in pendingBets) {
				bet_memos.Add($"home: {item.home_team_name}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"away: {item.away_team_name}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"start: {item.match_start_at.FormatDk()}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"market: {item.bet_market_name}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"odn: {item.bet_odd_name}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"odv: {item.bet_odd_value}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"bet: {item.bet_coin_amount} {item.bet_coin_name}".TruncateAsMetadataEntryDk());

				totalBetCoinAmount += item.bet_coin_amount;
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
			var sendAssets = new List<CardanoNode_AssetInfo>();
			if (betWithCoin.network != MstCurrencyModelConst.Network.Cardano) {
				return new ApiBadRequestResponse("Only ADA is supported for now. Not yet support other network");
			}
			// Bet with ADA
			if (betWithCoin.code == MstCurrencyModelConst.CODE_ADA) {
				sendAssets.Add(new() {
					asset_id = MstCurrencyModelConst.CODE_ADA,
					quantity = $"{(totalBetCoinAmount * AppConst.ADA_COIN2TOKEN):0}"
				});
			}
			// Bet with other token
			else {
				sendAssets.Add(new() {
					asset_id = betWithCoin.code,
					quantity = $"{(totalBetCoinAmount * DkMaths.Pow(10, betWithCoin.decimals)):0}"
				});
				// Attach 1.4 ADA for other token
				sendAssets.Add(new() {
					asset_id = MstCurrencyModelConst.CODE_ADA,
					quantity = $"{AppConst.MIN_LOVELACE_TO_SEND}"
				});
			}
			var cardanoRequest = new CardanoNode_SendAssetsRequestBody {
				sender_address = betUserWalletAddr,
				receiver_address = sysWalletAddr,
				fee_payer_address = betUserWalletAddr,
				discount_fee_from_assets = false,

				metadata = betMetadata,

				assets = sendAssets.ToArray()
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
			var submitStatus = txFailed ? SportUserBetModelConst.TxStatus.SubmitFailed : SportUserBetModelConst.TxStatus.SubmitSucceed;
			var submitMessage = txResultMessage.TruncateForShortLengthDk();

			foreach (var ubet in userBets) {
				ubet.submit_tx_status = submitStatus;
				ubet.submit_tx_id = txId;
				ubet.submit_tx_result_message = submitMessage;
			}
			await this.dbContext.SaveChangesAsync();
		}
	}

	public async Task<ApiResponse> GetBetHistories(Guid userId, int sport_id, int pagePos, int pageSize) {
		var query =
			from _ubet in this.dbContext.sportUserBets

			join _match in this.dbContext.sportMatches on _ubet.sport_match_id equals _match.id
			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			join _coin in this.dbContext.currencies on _ubet.bet_currency_id equals _coin.id
			join _country in this.dbContext.countries on _league.country_id equals _country.id into _left_countries
			from _country in _left_countries.DefaultIfEmpty()

			where _league.sport_id == sport_id
			where _ubet.user_id == userId

			orderby _ubet.id descending

			select new Sport_GetBetHistoriesResponse.Bet {
				ticket = _ubet.id,
				start_at = _match.start_at,

				country = _country.name,
				league = _league.name,

				team1 = _team1.name,
				team2 = _team2.name,

				score1 = _match.home_score,
				score2 = _match.away_score,

				status = _match.status,
				timer = _match.timer,

				bet_market_name = _ubet.bet_market_name,
				bet_odd_value = _ubet.bet_odd_value,
				staked = _ubet.bet_currency_amount,
				coin = _coin.name,
			}
		;

		var pagedResult = await query.AsNoTracking().PaginateDk(pagePos, pageSize);
		var bets = pagedResult.items;

		return new Sport_GetBetHistoriesResponse {
			data = new() {
				page_pos = pagedResult.pagePos,
				page_count = pagedResult.pageCount,
				total_item_count = pagedResult.totalItemCount,
				bets = bets
			}
		};
	}
}
