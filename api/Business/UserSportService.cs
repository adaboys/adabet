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
	private readonly SystemDao systemDao;
	private readonly RedisComponent redisComponent;

	public UserSportService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		CardanoNodeRepo cardanoNodeRepo,
		SystemDao systemDao,
		RedisComponent redisComponent
	) : base(dbContext, snapshot) {
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.systemDao = systemDao;
		this.redisComponent = redisComponent;
	}

	public async Task<ApiResponse> PlaceBet(Guid user_id, int sport_id, Sport_PlaceBetPayload payload) {
		// Validate req-bets (market + odds)
		// Target on each match -> target on each market -> target on each odd.
		var (req_totalBetCurrencyAmount, failureResponse) = await this._ValidateBets(sport_id, payload);
		if (failureResponse != null && failureResponse.failed) {
			return failureResponse;
		}

		// Since user will bet with internal wallet,
		// so first, we soft validate user balance.
		var userWalletAddress = await this.dbContext.userWallets
			.Where(m =>
				m.user_id == user_id &&
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

		var betWithCoin = await this.dbContext.currencies.FirstOrDefaultAsync(m => m.id == payload.bet_currency);
		if (betWithCoin is null) {
			return new ApiBadRequestResponse("Invalid bet currency");
		}
		var balanceResponse = await this.cardanoNodeRepo.GetMergedAssetsAsync(userWalletAddress);
		if (balanceResponse.failed) {
			return balanceResponse;
		}
		var holdCoinAmount = CardanoHelper.CalcCoinAmountFromAssets(betWithCoin, balanceResponse.data.assets);
		if (holdCoinAmount < req_totalBetCurrencyAmount) {
			return new ApiBadRequestResponse("User's balance does not enough to bet") { code = ErrCode.balance_not_enough };
		}

		// Validate requested reward address
		if (payload.reward_address != null) {
			if (!CardanoHelper.IsAddressMatchWithEnv(this.appSetting, payload.reward_address)) {
				return new ApiBadRequestResponse("Address does not match with current env") { code = ErrCode.invalid_address };
			}
			var validationResponse = await this.cardanoNodeRepo.ValidateAddressAsync(payload.reward_address);
			if (validationResponse.failed) {
				return validationResponse;
			}
		}

		var reward_receiver_address = payload.reward_address ?? userWalletAddress;

		// Register user bet and submit to blockchain
		var userBets = new List<SportUserBetModel>();
		foreach (var req_bet in payload.bets) {
			foreach (var req_market in req_bet.markets) {
				foreach (var req_odd in req_market.odds) {
					userBets.Add(new() {
						user_id = user_id,
						sport_match_id = req_bet.match_id,

						bet_market_name = req_market.name,
						bet_odd_name = req_odd.name,
						bet_odd_value = req_odd.value,

						bet_currency_id = betWithCoin.id,
						bet_currency_amount = req_odd.bet_amount,

						reward_receiver_address = reward_receiver_address
					});
				}
			}
		}
		this.dbContext.sportUserBets.AttachRange(userBets);
		await this.dbContext.SaveChangesAsync();

		// Increment bet count for statistics
		await this.dbContext.sportMatches.ExecuteUpdateAsync(s => s.SetProperty(m => m.user_bet_count, m => m.user_bet_count + 1));

		// Submit the bet to chain. And update user bets.
		return await this._SubmitBetsToChainAndUpdateTxStatusAsync(userBets, betWithCoin);
	}

	public async Task<ApiResponse> RequestPlaceBetViaExtWallet(Guid user_id, int sport_id, RequestBetViaExtWalletPayload payload) {
		// Validate req-bets (market + odds)
		// Target on each match -> target on each market -> target on each odd.
		var (req_totalBetCurrencyAmount, failureResponse) = await this._ValidateBets(sport_id, payload);
		if (failureResponse != null && failureResponse.failed) {
			return failureResponse;
		}

		// Cache request
		var orderCode = Guid.NewGuid().ToStringWithoutHyphen();
		var cacheKey = RedisKey.ForPlaceBetViaExtWallet(orderCode);
		var timeout = TimeSpan.FromDays(1);
		var cached = await this.redisComponent.SetJsonAsync(
			cacheKey,
			new RequestBetViaExtWalletCache {
				user_id = user_id,
				sport_id = sport_id,
				payload = payload
			},
			timeout
		);
		if (!cached) {
			return new ApiInternalServerErrorResponse("Could not store request");
		}

		var sysGameAddress = await this.systemDao.GetSystemGameAddressAsync();

		return new RequestBetViaExtWalletResponse {
			data = new() {
				order_code = orderCode,
				timeout = ((int)timeout.TotalSeconds),
				receiver_address = sysGameAddress,
				total_bet_amount = req_totalBetCurrencyAmount
			}
		};
	}

	public async Task<ApiResponse> ReportPlaceBetWithExtWallet(Guid user_id, int sport_id, ReportBetViaExtWalletPayload payload) {
		var cacheKey = RedisKey.ForPlaceBetViaExtWallet(payload.order_code);
		var cache = await this.redisComponent.GetJsonAsync<RequestBetViaExtWalletCache>(cacheKey);
		if (cache is null) {
			return new ApiBadRequestResponse("Request timeout");
		}

		// Validate requested sender address
		var rewardAddress = payload.sender_address;
		if (!CardanoHelper.IsAddressMatchWithEnv(this.appSetting, rewardAddress)) {
			return new ApiBadRequestResponse("Address does not match with current env") { code = ErrCode.invalid_address };
		}
		// MUST be linked to the account
		var linkedWalletName = await this.dbContext.userWallets
			.Where(m =>
				m.wallet_address == rewardAddress &&
				m.wallet_type == UserWalletModelConst.WalletType.External &&
				m.wallet_status == UserWalletModelConst.WalletStatus.Active
			)
			.Select(m => m.wallet_name)
			.FirstOrDefaultAsync()
		;
		if (linkedWalletName is null) {
			return new ApiBadRequestResponse("The wallet must be linked to user's account first");
		}

		var betWithCoinId = await this.dbContext.currencies.Where(m => m.id == cache.payload.bet_currency).Select(m => m.id).FirstAsync();

		// Register user bet and request for vefirying payment
		var userBets = new List<SportUserBetModel>();
		foreach (var requestBet in cache.payload.bets) {
			foreach (var requestMarket in requestBet.markets) {
				foreach (var requestOdd in requestMarket.odds) {
					var potentialReward = requestOdd.bet_amount * requestOdd.value;

					userBets.Add(new() {
						user_id = user_id,
						sport_match_id = requestBet.match_id,

						submit_tx_status = SportUserBetModelConst.SubmitTxStatus.RequestVerifyPaymentAtChain,
						submit_tx_hash = payload.payment_tx_hash,
						submit_tx_result_message = $"Bet via {linkedWalletName} ext-wallet",

						bet_market_name = requestMarket.name,
						bet_odd_name = requestOdd.name,
						bet_odd_value = requestOdd.value,

						bet_currency_id = betWithCoinId,
						bet_currency_amount = requestOdd.bet_amount,
						potential_reward_amount = potentialReward,

						reward_receiver_address = rewardAddress,
					});
				}
			}
		}
		this.dbContext.sportUserBets.AttachRange(userBets);
		await this.dbContext.SaveChangesAsync();

		// Increment bet count for statistics
		await this.dbContext.sportMatches.ExecuteUpdateAsync(s => s.SetProperty(m => m.user_bet_count, m => m.user_bet_count + 1));

		// Remove cache
		await this.redisComponent.DeleteKeyAsync(cacheKey);

		return new ApiSuccessResponse("Placed bet");
	}

	private async Task<(decimal, ApiResponse?)> _ValidateBets(int sport_id, Sport_BasePlaceBetPayload payload) {
		var req_totalBetCurrencyAmount = 0m;

		// Get target matches
		var targetMatchIds = payload.bets.Select(m => m.match_id).ToArray();
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
		var matches = await queryMatches.AsNoTracking().ToDictionaryAsync(m => m.match_id);
		if (matches.Count != payload.bets.Length) {
			return (req_totalBetCurrencyAmount, new ApiBadRequestResponse("The request contains invalid match status") { code = ErrCode.invalid_match });
		}

		foreach (var requestBet in payload.bets) {
			var match = matches.GetValueOrDefault(requestBet.match_id);
			if (match is null) {
				return (req_totalBetCurrencyAmount, new ApiBadRequestResponse($"Not found match {requestBet.match_id}") { code = ErrCode.not_found_match });
			}
			var markets = DkJsons.ToObj<List<Market>>(match.markets) ?? new();
			var name2market = markets.ToDictionary(m => m.name);
			if (name2market.Count == 0) {
				return (req_totalBetCurrencyAmount, new ApiBadRequestResponse($"Market of the match is unavailable for now") { code = ErrCode.market_unavailable });
			}

			// Target on each market.
			// Each req-market must found in db-markets of the match
			foreach (var requestMarket in requestBet.markets) {
				var market = name2market.GetValueOrDefault(requestMarket.name);
				if (market is null) {
					return (req_totalBetCurrencyAmount, new ApiBadRequestResponse($"Not found market {requestMarket.name}") { code = ErrCode.not_found_market });
				}

				// Target on each odd in the market.
				// Each req-odd must found in db-odds of the match's market
				var name2odd = market.odds.ToDictionary(m => m.name);
				foreach (var requestOdd in requestMarket.odds) {
					var odd = name2odd.GetValueOrDefault(requestOdd.name);
					if (odd is null) {
						return (req_totalBetCurrencyAmount, new ApiBadRequestResponse($"Not found odd {requestOdd.name}") { code = ErrCode.not_found_odd });
					}
					if (odd.suspend) {
						return (req_totalBetCurrencyAmount, new ApiBadRequestResponse($"Current odd {odd.name} is unavailable for betting"));
					}
					if (odd.value != requestOdd.value) {
						return (req_totalBetCurrencyAmount, new ApiBadRequestResponse("Odd changed, please reload to try with new bet") { code = ErrCode.odd_changed });
					}

					req_totalBetCurrencyAmount += requestOdd.bet_amount;
				}
			}
		}

		return (req_totalBetCurrencyAmount, null);
	}

	private async Task<ApiResponse> _SubmitBetsToChainAndUpdateTxStatusAsync(List<SportUserBetModel> userBets, MstCurrencyModel betWithCoin) {
		var txFailed = true;
		string? submitTxId = null;
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
			var sysWalletAddr = await this.systemDao.GetSystemGameAddressAsync();
			var totalBetCoinAmount = 0m;
			string? userInternalWalletAddress = null;

			foreach (var item in pendingBets) {
				bet_memos.Add($"home: {item.home_team_name}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"away: {item.away_team_name}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"start: {item.match_start_at.FormatDk()}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"market: {item.bet_market_name}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"odn: {item.bet_odd_name}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"odv: {item.bet_odd_value.TrimEndDecimalZeroDk()}".TruncateAsMetadataEntryDk());
				bet_memos.Add($"staked: {item.bet_coin_amount.TrimEndDecimalZeroDk()} {item.bet_coin_name}".TruncateAsMetadataEntryDk());

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
				// Need 1.4 ADA when send non-ADA token
				sendAssets.Add(new() {
					asset_id = MstCurrencyModelConst.CODE_ADA,
					quantity = $"{AppConst.MIN_LOVELACE_TO_SEND}"
				});

				sendAssets.Add(new() {
					asset_id = betWithCoin.code,
					quantity = $"{(totalBetCoinAmount * DkMaths.Pow(10, betWithCoin.decimals)):0}"
				});
			}

			var cnodeRequest = new CardanoNode_SendAssetsRequestBody {
				sender_address = betUserWalletAddr,
				receiver_address = sysWalletAddr,
				fee_payer_address = betUserWalletAddr,
				discount_fee_from_assets = false,

				metadata = betMetadata,

				assets = sendAssets.ToArray()
			};

			// Send to chain and Update status
			var cnodeResponse = await this.cardanoNodeRepo.SendAssetsAsync(cnodeRequest);

			txFailed = cnodeResponse.failed;
			submitTxId = cnodeResponse.data?.tx_id;
			txResultMessage = cnodeResponse.message;

			return txFailed ?
				new ApiInternalServerErrorResponse(txResultMessage) { code = cnodeResponse.code } :
				new ApiSuccessResponse(txResultMessage)
			;
		}
		catch (Exception e) {
			txResultMessage = e.Message;

			return new ApiInternalServerErrorResponse("Could not submit bets to chain");
		}
		// Update tx-status at finally block
		finally {
			var submitTxStatus = txFailed ? SportUserBetModelConst.SubmitTxStatus.SubmitToChainFailed : SportUserBetModelConst.SubmitTxStatus.SubmitToChainSucceed;

			foreach (var ubet in userBets) {
				ubet.submit_tx_status = submitTxStatus;
				ubet.submit_tx_hash = submitTxId;
				ubet.submit_tx_result_message = txResultMessage;
			}
			await this.dbContext.SaveChangesAsync();
		}
	}

	public async Task<ApiResponse> GetBetHistories(Guid userId, int sport_id, int pagePos, int pageSize, string? tab) {
		var notFilterBetResult = (tab is null);
		var bet_result = tab switch {
			"won" => SportUserBetModelConst.BetResult.Won,
			"lost" => SportUserBetModelConst.BetResult.Losed,
			"draw" => SportUserBetModelConst.BetResult.Draw,
			_ => SportUserBetModelConst.BetResult.Nothing
		};

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
			where notFilterBetResult || _ubet.bet_result == bet_result

			orderby _ubet.id descending

			select new Sport_GetBetHistoriesResponse.Bet {
				sport_id = _league.sport_id,
				ticket = _ubet.id,
				start_at = _match.start_at,
				status = (int)_match.status,

				country = _country.name,
				league = _league.name,

				team1 = _team1.name,
				team2 = _team2.name,

				score1 = _match.home_score,
				score2 = _match.away_score,

				timer = _match.timer,

				bet_market_name = _ubet.bet_market_name,
				bet_odd_value = _ubet.bet_odd_value,
				coin_name = _coin.name,
				staked = _ubet.bet_currency_amount,
				potential_reward = _ubet.potential_reward_amount,

				bet_result = (int)_ubet.bet_result,
				bet_tx_id = _ubet.submit_tx_hash,

				reward_status = (int?)(_ubet.reward_tx_status) ?? 0,
				reward_tx_id = _ubet.reward_tx_hash,
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

	public async Task<ApiResponse> GetBadges(Guid user_id, int sport_id) {
		var queryFavorite =
			from _fav in this.dbContext.userFavoriteMatches

			join _mat in this.dbContext.sportMatches on _fav.match_id equals _mat.id
			join _leg in this.dbContext.sportLeagues on _mat.league_id equals _leg.id

			where _fav.user_id == user_id
			where _leg.sport_id == sport_id
			where SportMatchModelConst.ActiveTimeStatusForFavorite.Contains(_mat.status)

			select new { }
		;
		var favCount = await queryFavorite.CountAsync();

		var queryBet =
			from _bet in this.dbContext.sportUserBets

			join _mat in this.dbContext.sportMatches on _bet.sport_match_id equals _mat.id
			join _leg in this.dbContext.sportLeagues on _mat.league_id equals _leg.id

			where _bet.user_id == user_id
			where _leg.sport_id == sport_id
			where SportMatchModelConst.ActiveTimeStatusForBadges.Contains(_mat.status)

			select new { }
		;
		var betCount = await queryBet.CountAsync();

		return new GetBadgesResponse {
			data = new() {
				favorite_count = favCount,
				bet_count = betCount
			}
		};
	}

	public class RequestBetViaExtWalletCache {
		public Guid user_id { get; set; }
		public int sport_id { get; set; }
		public RequestBetViaExtWalletPayload payload { get; set; }
	}
}
