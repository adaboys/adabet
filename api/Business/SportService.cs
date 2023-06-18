namespace App;

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tool.Compet.EntityFrameworkCore;
using Tool.Compet.Json;

public class SportService {
	private readonly AppDbContext dbContext;

	public SportService(AppDbContext dbContext) {
		this.dbContext = dbContext;
	}

	public async Task<ApiResponse> GetSportList() {
		var sports = await this.dbContext.sports.AsNoTracking()
			.Select(m => new Sport_ListResponse.Sport {
				id = m.id,
				name = m.name,
				//todo impl it
				country_count = Random.Shared.Next(100)
			})
			.ToArrayAsync()
		;

		return new Sport_ListResponse {
			data = new() {
				sports = sports
			}
		};
	}

	public async Task<ApiResponse> GetQuickLinkOfLeagues() {
		return new Sport_GetQuickLinksResponse {
			data = new() {
				links = new Sport_GetQuickLinksResponse.Link[] {
					new() { name = "World Cup" },
					new() { name = "Premier League" },
					new() { name = "LaLiga" },
					new() { name = "Serie A" },
					new() { name = "NBA" },
					new() { name = "Seagame" },
				}
			}
		};
	}

	public async Task<ApiResponse> GetLiveMatches(Guid? user_id, int sport_id) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			join _country in this.dbContext.countries on _league.country_id equals _country.id into _left_countries
			from _country in _left_countries.DefaultIfEmpty()

			where _league.sport_id == sport_id
			where _match.status == SportMatchModelConst.TimeStatus.InPlay
			where _match.lock_mode == SportMatchModelConst.LockMode.Nothing

			orderby _match.start_at ascending

			select new Sport_MatchesResponse.Match {
				id = _match.id,
				start_at = _match.start_at,

				country = _country.name,
				league = _league.name,

				is_esport = _match.is_esport,

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

		var matches = await query.AsNoTracking().ToArrayAsync();

		// Check whether each match is in user's favorited matches
		if (user_id != null) {
			var matchIds = matches.Select(m => m.id).ToArray();
			var userFavMatchIds = await this.dbContext.userFavoriteMatches
				.Where(m => m.user_id == user_id)
				.Where(m => m.toggled)
				.Where(m => matchIds.Contains(m.match_id))
				.Select(m => m.match_id)
				.ToArrayAsync()
			;
			var uniqueFavMatchIds = userFavMatchIds.ToHashSet();

			foreach (var mat in matches) {
				mat.favorited = uniqueFavMatchIds.Contains(mat.id);
			}
		}

		return new Sport_MatchesResponse {
			data = new() {
				matches = matches
			}
		};
	}

	public async Task<ApiResponse> GetUpcomingMatches(Guid? user_id, int sport_id, int pagePos, int pageSize) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			join _country in this.dbContext.countries on _league.country_id equals _country.id into _left_countries
			from _country in _left_countries.DefaultIfEmpty()

			where _league.sport_id == sport_id
			where _match.status == SportMatchModelConst.TimeStatus.Upcoming
			where _match.lock_mode == SportMatchModelConst.LockMode.Nothing
			where _match.markets != null // Ignore matches that does not ready for betting

			orderby _match.start_at ascending

			select new Sport_UpcomingMatchesResponse.Match {
				id = _match.id,
				start_at = _match.start_at,

				country = _country.name,
				league = _league.name,

				is_esport = _match.is_esport,

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

		// Check whether each match is in user's favorited matches
		if (user_id != null) {
			var matchIds = matches.Select(m => m.id).ToArray();
			var userFavMatchIds = await this.dbContext.userFavoriteMatches
				.Where(m => m.user_id == user_id)
				.Where(m => m.toggled)
				.Where(m => matchIds.Contains(m.match_id))
				.Select(m => m.match_id)
				.ToArrayAsync()
			;
			var uniqueFavMatchIds = userFavMatchIds.ToHashSet();

			foreach (var mat in matches) {
				mat.favorited = uniqueFavMatchIds.Contains(mat.id);
			}
		}

		return new Sport_UpcomingMatchesResponse {
			data = new() {
				page_pos = pagedResult.pagePos,
				page_count = pagedResult.pageCount,
				total_item_count = pagedResult.totalItemCount,
				matches = matches
			}
		};
	}

	public async Task<ApiResponse> GetHighlightMatches(int sport_id) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			join _country in this.dbContext.countries on _league.country_id equals _country.id into _left_countries
			from _country in _left_countries.DefaultIfEmpty()

			where ((int)_league.sport_id) == sport_id
			where _match.status == SportMatchModelConst.TimeStatus.InPlay
			where _match.lock_mode == SportMatchModelConst.LockMode.Nothing

			orderby _match.user_bet_count descending, _match.start_at ascending

			select new Sport_GetHighlightMatchesResponse.Match {
				id = _match.id,
				start_at = _match.start_at,

				country = _country.name,
				league = _league.name,

				is_esport = _match.is_esport,

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

				tmp_markets = _match.markets == null ? null : DkJsons.ToObj<List<Market>>(_match.markets!),
			}
		;

		var matches = await query.AsNoTracking().Take(20).ToArrayAsync();

		// Shape data
		foreach (var item in matches) {
			if (item.tmp_markets != null) {
				item.market = item.tmp_markets.FirstOrDefault(m => m.name == MarketConst.MainFullTime);
			}
		}

		return new Sport_GetHighlightMatchesResponse {
			data = new() {
				matches = matches
			}
		};
	}

	public async Task<ApiResponse> GetTopMatches(Guid? user_id, int sport_id) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			join _country in this.dbContext.countries on _league.country_id equals _country.id into _left_countries
			from _country in _left_countries.DefaultIfEmpty()

			where _league.sport_id == sport_id
			where _match.status == SportMatchModelConst.TimeStatus.InPlay
			where _match.lock_mode == SportMatchModelConst.LockMode.Nothing

			orderby _match.start_at ascending

			select new Sport_GetTopMatchesResponse.Match {
				id = _match.id,
				start_at = _match.start_at,

				country = _country.name,
				league = _league.name,

				is_esport = _match.is_esport,

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

		var matches = await query.AsNoTracking().Take(20).ToArrayAsync();

		// Check whether each match is in user's favorited matches
		if (user_id != null) {
			var matchIds = matches.Select(m => m.id).ToArray();
			var userFavMatchIds = await this.dbContext.userFavoriteMatches
				.Where(m => m.user_id == user_id)
				.Where(m => m.toggled)
				.Where(m => matchIds.Contains(m.match_id))
				.Select(m => m.match_id)
				.ToArrayAsync()
			;
			var uniqueFavMatchIds = userFavMatchIds.ToHashSet();

			foreach (var mat in matches) {
				mat.favorited = uniqueFavMatchIds.Contains(mat.id);
			}
		}

		return new Sport_GetTopMatchesResponse {
			data = new() {
				matches = matches
			}
		};
	}
}
