namespace App;

using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tool.Compet.Core;
using Tool.Compet.EntityFrameworkCore;
using Tool.Compet.Json;

/// Raw query with interpolated: https://docs.microsoft.com/en-us/ef/core/querying/raw-sql
public class UserFavoriteService : BaseService {
	private readonly ILogger<UserFavoriteService> logger;
	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly UserDao userDao;
	private readonly ApiNodejsRepo apiNodejsRepo;
	private readonly RedisComponent redisService;

	public UserFavoriteService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UserFavoriteService> logger,
		CardanoNodeRepo cardanoNodeRepo,
		UserDao userDao,
		ApiNodejsRepo apiNodejsRepo,
		RedisComponent redisService
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.userDao = userDao;
		this.apiNodejsRepo = apiNodejsRepo;
		this.redisService = redisService;
		this.cardanoNodeRepo = cardanoNodeRepo;
	}

	public async Task<ApiResponse> ToggleFavoriteMatch(Guid user_id, long match_id, bool toggle_on) {
		// Get match
		var match = await this.dbContext.sportMatches.FirstOrDefaultAsync(m => m.id == match_id);
		if (match is null) {
			return new ApiBadRequestResponse("Invalid match");
		}

		var favoriteMatch = await this.dbContext.userFavoriteMatches.FirstOrDefaultAsync(m =>
			m.user_id == user_id &&
			m.match_id == match_id
		);

		if (favoriteMatch is null) {
			if (!toggle_on) {
				return new ApiBadRequestResponse("Not found favorite on the match");
			}

			favoriteMatch = new() {
				user_id = user_id,
				match_id = match_id
			};

			this.dbContext.userFavoriteMatches.Attach(favoriteMatch);
		}

		favoriteMatch.toggled = toggle_on;

		// It is useful to let us know how the match is favorited
		match.favorite_count = toggle_on ? match.favorite_count + 1 : match.favorite_count - 1;

		await this.dbContext.SaveChangesAsync();

		return new ApiSuccessResponse(toggle_on ? "Toggled" : "Untoggled");
	}

	public async Task<ApiResponse> GetListOfFavoriteMatch(Guid user_id, int sport_id, int pagePos, int pageSize) {
		var query =
			from _favorite in this.dbContext.userFavoriteMatches

			join _match in this.dbContext.sportMatches on _favorite.match_id equals _match.id

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			join _country in this.dbContext.countries on _league.country_id equals _country.id into _left_countries
			from _country in _left_countries.DefaultIfEmpty()

			where _league.sport_id == sport_id
			where _favorite.toggled == true
			where SportMatchModelConst.ActiveTimeStatusForFavorite.Contains(_match.status)

			select new GetListOfFavoriteMatchResponse.Match {
				id = _match.id,
				sport_id = _league.sport_id,
				start_at = _match.start_at,

				country = _country.name,
				league = _league.name,

				is_esport = _match.is_esport,

				status = (int)_match.status,

				team1 = _team1.name,
				team2 = _team2.name,

				image1 = SportTeamModelConst.CalcFlagImageName(_team1.flag_image_name, _team1.flag_image_src),
				image2 = SportTeamModelConst.CalcFlagImageName(_team2.flag_image_name, _team2.flag_image_src),

				score1 = _match.home_score,
				score2 = _match.away_score,

				timer = new() {
					time = _match.timer,
					total_timer = _match.total_timer,
					is_break = _match.timer_break,
					injury_time = _match.timer_injury,
				},

				markets = _match.markets == null ? null : DkJsons.ToObj<List<Market>>(_match.markets!),
			}
		;

		var pagedResult = await query.AsNoTracking().PaginateDk(pagePos, pageSize);
		var matches = pagedResult.items;

		return new GetListOfFavoriteMatchResponse {
			data = new() {
				page_pos = pagedResult.pagePos,
				page_count = pagedResult.pageCount,
				total_item_count = pagedResult.totalItemCount,
				matches = matches
			}
		};
	}
}
