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

	public async Task<ApiResponse> GetSportPredictionMatches(int sport_id, int pagePos, int pageSize) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			join _country in this.dbContext.countries on _league.country_id equals _country.id into _left_countries
			from _country in _left_countries.DefaultIfEmpty()

			where _league.sport_id == sport_id
			where _match.user_can_predict_free == true

			orderby _match.start_at descending

			select new GetSportPredictionMatchesResponse.Match {
				id = _match.id,

				start_at = _match.start_at,

				country = _country.name_en,
				league = _league.name,

				team1 = _team1.name,
				team2 = _team2.name,

				image1 = SportTeamModelConst.CalcFlagImageName(_team1.flag_image_name, _team1.flag_image_src),
				image2 = SportTeamModelConst.CalcFlagImageName(_team2.flag_image_name, _team2.flag_image_src),

				score1 = _match.home_score,
				score2 = _match.away_score,

				cur_time = _match.cur_play_time,
			}
		;

		var pagedResult = await query.AsNoTracking().PaginateDk(pagePos, pageSize);
		var matches = pagedResult.items;

		return new GetSportPredictionMatchesResponse {
			data = new() {
				page_pos = pagedResult.pagePos,
				page_count = pagedResult.pageCount,
				total_item_count = pagedResult.totalItemCount,
				matches = matches
			}
		};
	}

	public async Task<ApiResponse> PredictMatch(Guid userId, long match_id, Sport_PredictMatchRequestBody reqBody) {
		var match = await this.dbContext.sportMatches.FirstOrDefaultAsync(m =>
			m.id == match_id &&
			m.status == SportMatchModelConst.TimeStatus.Upcoming
		);
		if (match is null) {
			return new ApiBadRequestResponse("The match is unavailable for predict");
		}

		// Only onetime prediction
		var predict = await this.dbContext.sportPredictUsers.FirstOrDefaultAsync(m =>
			m.user_id == userId &&
			m.sport_match_id == match_id
		);
		if (predict != null) {
			return new ApiBadRequestResponse("Already predicted");
		}

		var reward_address = reqBody.reward_address;
		if (reward_address is null) {
			reward_address = await this.dbContext.userWallets
				.Where(m =>
					m.user_id == userId &&
					m.wallet_type == UserWalletModelConst.WalletType.Internal &&
					m.wallet_status == UserWalletModelConst.WalletStatus.Active
				)
				.Select(m => m.wallet_address)
				.FirstAsync();
		}

		this.dbContext.sportPredictUsers.Attach(new() {
			user_id = userId,
			sport_match_id = match_id,
			predict_home_score = reqBody.score1,
			predict_away_score = reqBody.score2,
			reward_address = reward_address
		});

		await this.dbContext.SaveChangesAsync();

		return new ApiSuccessResponse();
	}

	public async Task<ApiResponse> GetSportMatchPredictedUsers(long match_id, int pagePos, int pageSize) {
		var query =
			from _predict in this.dbContext.sportPredictUsers

			join _match in this.dbContext.sportMatches on _predict.sport_match_id equals _match.id
			join _user in this.dbContext.users on _predict.user_id equals _user.id
			join _coin in this.dbContext.cardanoCoins on _predict.rewarded_coin_id equals _coin.id

			where _match.id == match_id
			where _match.user_can_predict_free == true

			orderby _predict.prediction_result_rank ascending, _predict.created_at ascending

			select new GetSportPredictionUserListOnMatchResponse.Prediction {
				player_name = _user.player_name,
				score1 = _predict.predict_home_score,
				score2 = _predict.predict_away_score,
				predicted_at = _predict.created_at,

				rewarded_coin_name = _coin.coin_name,
				rewarded_coin_amount = _predict.rewarded_coin_amount,
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
}
