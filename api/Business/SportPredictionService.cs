namespace App;

using System;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tool.Compet.Core;
using Tool.Compet.EntityFrameworkCore;
using Tool.Compet.Json;

public class SportPredictionService : BaseService {
	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly SystemWalletDao systemWalletDao;

	public SportPredictionService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		CardanoNodeRepo cardanoNodeRepo,
		SystemWalletDao systemWalletDao
	) : base(dbContext, snapshot) {
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.systemWalletDao = systemWalletDao;
	}

	public async Task<ApiResponse> GetPredictionMatchList(Guid? userId, int sport_id, int pagePos, int pageSize) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			join _country in this.dbContext.countries on _league.country_id equals _country.id into _left_countries
			from _country in _left_countries.DefaultIfEmpty()

			where _league.sport_id == sport_id
			where _match.predictable == true
			where _match.lock_mode == SportMatchModelConst.LockMode.Nothing

			orderby _match.start_at descending

			select new GetSportPredictionMatchesResponse.Match {
				id = _match.id,

				start_at = _match.start_at,

				country = _country.name,
				league = _league.name,

				team1 = _team1.name,
				team2 = _team2.name,

				image1 = SportTeamModelConst.CalcFlagImageName(_team1.flag_image_name, _team1.flag_image_src),
				image2 = SportTeamModelConst.CalcFlagImageName(_team2.flag_image_name, _team2.flag_image_src),

				score1 = _match.home_score,
				score2 = _match.away_score,

				timer = _match.timer,
			}
		;

		var pagedResult = await query.AsNoTracking().PaginateDk(pagePos, pageSize);
		var matches = pagedResult.items;

		// Check the user has predicted or not on each match.
		if (userId != null) {
			var predictedMatches = await this.dbContext.sportPredictUsers
				.Where(m => m.user_id == userId && m.predicted_at != null)
				.Select(m => new { m.sport_match_id, m.predicted_at })
				.ToDictionaryAsync(m => m.sport_match_id)
			;
			foreach (var match in matches) {
				match.predicted_at = predictedMatches.GetValueOrDefault(match.id)?.predicted_at;
			}
		}

		// Count participant joined to predict on each match
		var match_participant = await this.dbContext.sportPredictUsers
			.Where(m => matches.Select(m => m.id).ToArray().Contains(m.sport_match_id))
			.GroupBy(m => m.sport_match_id)
			.Select(g => new {
				match_id = g.Key,
				participant_count = g.Count(),
			})
			.ToDictionaryAsync(m => m.match_id)
		;
		foreach (var match in matches) {
			match.participant = match_participant.GetValueOrDefault(match.id)?.participant_count ?? 0;
		}

		return new GetSportPredictionMatchesResponse {
			data = new() {
				page_pos = pagedResult.pagePos,
				page_count = pagedResult.pageCount,
				total_item_count = pagedResult.totalItemCount,
				matches = matches
			}
		};
	}

	public async Task<ApiResponse> GetPredictionMatchDetail(long match_id) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			join _country in this.dbContext.countries on _league.country_id equals _country.id into _left_countries
			from _country in _left_countries.DefaultIfEmpty()

			where _match.id == match_id

			select new GetSportPredictionMatchDetailResponse.Match {
				id = _match.id,

				start_at = _match.start_at,

				country = _country.name,
				league = _league.name,

				team1 = _team1.name,
				team2 = _team2.name,

				image1 = SportTeamModelConst.CalcFlagImageName(_team1.flag_image_name, _team1.flag_image_src),
				image2 = SportTeamModelConst.CalcFlagImageName(_team2.flag_image_name, _team2.flag_image_src),

				score1 = _match.home_score,
				score2 = _match.away_score,

				timer = _match.timer,
			}
		;

		var match = await query.AsNoTracking().FirstOrDefaultAsync();
		if (match is null) {
			return new ApiBadRequestResponse("Invalid match");
		}

		// var winnerCount = await this.dbContext.sportPredictUsers
		// 	.Where(m => m.sport_match_id == match_id && )
		// 	.CountAsync()
		// ;

		match.winner = 0;
		match.loser = 0;

		return new GetSportPredictionMatchDetailResponse {
			data = new() {
				match = match
			}
		};
	}

	public async Task<ApiResponse> PredictMatch(Guid userId, long match_id, Sport_PredictMatchRequestBody reqBody) {
		// Check the match
		var match = await this.dbContext.sportMatches.FirstOrDefaultAsync(m =>
			m.id == match_id &&
			m.status == SportMatchModelConst.TimeStatus.Upcoming &&
			m.predictable == true &&
			m.lock_mode == SportMatchModelConst.LockMode.Nothing
		);
		if (match is null) {
			return new ApiBadRequestResponse("The match is unavailable for predict");
		}

		// Validate the reward address
		if (reqBody.reward_address != null) {
			var validationResponse = await this.cardanoNodeRepo.ValidateAddressAsync(reqBody.reward_address);
			if (validationResponse.failed) {
				return validationResponse;
			}
		}

		var userInternalWallet = await this.dbContext.userWallets
			.Where(m =>
				m.user_id == userId &&
				m.wallet_type == UserWalletModelConst.WalletType.Internal &&
				m.wallet_status == UserWalletModelConst.WalletStatus.Active
			)
			.Select(m => m.wallet_address)
			.FirstAsync()
		;

		// Check wallet balance (at least 1.6 ADA)
		var balanceResponse = await this.cardanoNodeRepo.GetMergedAssetsAsync(userInternalWallet);
		if (balanceResponse.failed) {
			return balanceResponse;
		}
		var userHoldAdaAmount = CardanoHelper.CalcTotalAdaFromAssets(balanceResponse.data.assets);
		if (userHoldAdaAmount < 1.6m) {
			return new ApiBadRequestResponse("Need at least 1.6 ADA for tx") { code = ErrCode.balance_not_enough };
		}

		// For now, we support only ADA as reward coin.
		// In future, user may request reward with GEM, ABE,...
		var rewardCoin = await this.dbContext.currencies
			.Where(m => m.network == MstCurrencyModelConst.Network.Cardano)
			.Select(m => new { m.id, m.name })
			.FirstAsync();

		// Step 1/2. Register prediction if not exist (allow only one bet)
		var prediction = await this.dbContext.sportPredictUsers.FirstOrDefaultAsync(m =>
			m.user_id == userId &&
			m.sport_match_id == match_id
		);
		if (prediction != null && SportPredictUserModelConst.SuccessBetSubmitStatusList.Contains(prediction.bet_submit_tx_status)) {
			return new ApiBadRequestResponse("Already predicted");
		}
		if (prediction is null) {
			var reward_address = reqBody.reward_address ?? userInternalWallet;

			prediction = new() {
				user_id = userId,
				sport_match_id = match_id,
				predict_home_score = reqBody.score1,
				predict_away_score = reqBody.score2,
				reward_address = reward_address,
				reward_coin_id = rewardCoin.id
			};

			this.dbContext.sportPredictUsers.Attach(prediction);

			await this.dbContext.SaveChangesAsync();
		}

		// Get match info
		var queryMatchInfo =
			from _predict in this.dbContext.sportPredictUsers

			join _match in this.dbContext.sportMatches on _predict.sport_match_id equals _match.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id

			select new {
				home_team_name = _team1.name,
				away_team_name = _team2.name,
				match_start_at = _match.start_at
			}
		;
		var matchInfo = await queryMatchInfo.FirstAsync();

		// Step 2/2. Submit the bet to chain if not yet or previously failed.
		using (var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
			// Lock on the prediction
			prediction = await this.dbContext.sportPredictUsers
				.FromSqlRaw($"SELECT * FROM [{DbConst.table_sport_predict_user}] WITH (UPDLOCK) WHERE id = {{0}}", prediction.id)
				.FirstAsync()
			;

			// After locked, check bet-submit status
			if (SportPredictUserModelConst.SuccessBetSubmitStatusList.Contains(prediction.bet_submit_tx_status)) {
				return new ApiBadRequestResponse("Already submitted");
			}

			// Submit prediction to chain.
			// Let user predict with ADA. Just send 1.4 ADA to the user itself to indicate the user has predicted.
			var sendAssets = new CardanoNode_AssetInfo[] {
				new() {
					asset_id = MstCurrencyModelConst.CODE_ADA,
					quantity = $"{AppConst.MIN_LOVELACE_TO_SEND}"
				}
			};
			// We use CIP-20 (datum label as 674) to attach user's bet as message
			// Ref: https://cips.cardano.org/cips/cip20
			var bet_memos = new string[] {
				$"t1: {matchInfo.home_team_name}".TruncateAsMetadataEntryDk(),
				$"t2: {matchInfo.away_team_name}".TruncateAsMetadataEntryDk(),
				$"predict: {reqBody.score1} - {reqBody.score2}".TruncateAsMetadataEntryDk(),
				$"start: {matchInfo.match_start_at.FormatDk()}".TruncateAsMetadataEntryDk(),
				$"reward_as: {rewardCoin.name}".TruncateAsMetadataEntryDk(),
			};
			var cnodeRequest = new CardanoNode_SendAssetsRequestBody {
				sender_address = userInternalWallet,
				receiver_address = userInternalWallet,
				fee_payer_address = userInternalWallet,
				discount_fee_from_assets = false,
				assets = sendAssets,
				metadata = new BetMetadata {
					cip_674 = new() {
						msg_list = bet_memos
					}
				}
			};
			var cnodeResponse = await this.cardanoNodeRepo.SendAssetsAsync(cnodeRequest);

			// Update bet-submit
			prediction.bet_submit_tx_id = cnodeResponse.data?.tx_id;
			prediction.bet_submit_tx_result_message = cnodeResponse.message.TruncateForShortLengthDk();
			prediction.bet_submit_tx_status = cnodeResponse.succeed ? SportPredictUserModelConst.BetSubmitTxStatus.SubmitSucceed : SportPredictUserModelConst.BetSubmitTxStatus.SubmitFailed;

			if (cnodeResponse.succeed) {
				prediction.predicted_at = DateTime.UtcNow;
			}

			await this.dbContext.SaveChangesAsync();
			txScope.Complete();

			return cnodeResponse.succeed ? new ApiSuccessResponse() : cnodeResponse;
		};
	}

	public async Task<ApiResponse> GetPredictedUsersOnMatch(long match_id, int pagePos, int pageSize) {
		var query =
			from _predict in this.dbContext.sportPredictUsers

			join _match in this.dbContext.sportMatches on _predict.sport_match_id equals _match.id
			join _user in this.dbContext.users on _predict.user_id equals _user.id
			join _coin in this.dbContext.currencies on _predict.reward_coin_id equals _coin.id

			where _match.id == match_id
			where _match.predictable == true
			where SportPredictUserModelConst.SuccessBetSubmitStatusList.Contains(_predict.bet_submit_tx_status)

			orderby _predict.prediction_rank ascending, _predict.created_at ascending

			select new GetSportPredictionUserListOnMatchResponse.Prediction {
				player_name = _user.player_name,
				predict_score1 = _predict.predict_home_score,
				predict_score2 = _predict.predict_away_score,

				predicted_at = _predict.predicted_at,
				prediction_rank = _predict.prediction_rank,

				reward_coin_name = _coin.name,
				reward_coin_amount = _predict.reward_coin_amount,
				reward_delivery_status = ((int)_predict.reward_submit_tx_status),
			}
		;

		var pagedResult = await query.AsNoTracking().PaginateDk(pagePos, pageSize);
		var predictions = pagedResult.items;

		return new GetSportPredictionUserListOnMatchResponse {
			data = new() {
				page_pos = pagedResult.pagePos,
				page_count = pagedResult.pageCount,
				total_item_count = pagedResult.totalItemCount,
				predictions = predictions
			}
		};
	}

	/// @param upToNowSeconds: 0 for total.
	public async Task<ApiResponse> GetLeaderboard(int coin_id, long upToNowSeconds, int pagePos, int pageSize) {
		var leftmost = DateTime.UtcNow.AddSeconds(-upToNowSeconds);

		var predictionRanks = await this.dbContext.sportPredictUsers
			// For whereif, should write in new clause to avoid mistake of wrapping with bracket.
			.Where(m => m.reward_coin_id == coin_id)
			.Where(m => upToNowSeconds == 0 ? true : m.created_at >= leftmost)
			.Select(m => new { user_id = m.user_id, reward_coin_amount = m.reward_coin_amount })
			.GroupBy(m => m.user_id)
			.Select(g => new GetLeaderboardResponse.LeaderboardItem {
				tmp_userId = g.Key,
				reward_sum = g.Sum(m => m.reward_coin_amount),
			})
			.OrderByDescending(m => m.reward_sum)
			.Take(10)
			.ToArrayAsync()
		;

		if (predictionRanks.Length == 0) {
			return new ApiSuccessResponse("Empty");
		}

		var userIds = predictionRanks.Select(m => m.tmp_userId).ToArray();
		var users = await this.dbContext.users
			.Where(m => userIds.Contains(m.id))
			.Select(m => new { m.id, m.player_name, m.avatar_relative_path })
			.ToDictionaryAsync(m => m.id)
		;

		foreach (var item in predictionRanks) {
			var user = users.GetValueOrDefault(item.tmp_userId);

			item.player = user?.player_name;
			item.ava = user?.avatar_relative_path is null ? null : $"{this.appSetting.s3.baseUrl}/{user.avatar_relative_path}";
		}

		return new GetLeaderboardResponse {
			data = new() {
				leaderboard = predictionRanks
			}
		};
	}
}
