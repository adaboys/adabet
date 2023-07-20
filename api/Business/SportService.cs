namespace App;

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tool.Compet.Core;
using Tool.Compet.EntityFrameworkCore;
using Tool.Compet.Json;

public class SportService {
	private readonly AppDbContext dbContext;
	private BetsapiRepo betsapiRepo;

	public SportService(
		AppDbContext dbContext,
		BetsapiRepo betsapiRepo
	) {
		this.dbContext = dbContext;
		this.betsapiRepo = betsapiRepo;
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
				status = (int)_match.status,

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
				status = (int)_match.status,

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
				status = (int)_match.status,

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
				status = (int)_match.status,

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

	public async Task<ApiResponse> GetMatchHistory(long match_id) {
		var queryMatch =
			from _match in this.dbContext.sportMatches
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			where _match.id == match_id
			select new {
				ref_betsapi_match_id = _match.ref_betsapi_match_id,
				ref_betsapi_home_team_id = _team1.ref_betsapi_home_team_id,
				ref_betsapi_away_team_id = _team2.ref_betsapi_home_team_id,
			}
		;
		var matchResult = await queryMatch.FirstOrDefaultAsync();
		if (matchResult is null) {
			return new ApiBadRequestResponse("No match");
		}
		var response = await this.betsapiRepo.FetchMatchHistory(matchResult.ref_betsapi_match_id);
		if (response is null || response.failed) {
			return new ApiInternalServerErrorResponse("No data");
		}

		var last_matches = new Sport_GetMatchHistoryResponse.Matches();
		var next_matches = new Sport_GetMatchHistoryResponse.Matches();

		// Build home's matches (last, next)
		for (var index = response.results.home.Count - 1; index >= 0; --index) {
			var item = response.results.home[index];
			var status = SportMatchModelConst.ConvertStatusFromBetsapi(item.time_status);

			var homeMatch = new Sport_GetMatchHistoryResponse.Home() {
				league = item.league.name,
				time = item.time,
				status = (int)status,
				scores = item.ss,
				home = new() {
					name = item.home.name,
					img = item.home.image_id
				},
				away = new() {
					name = item.away.name,
					img = item.away.image_id
				}
			};

			if (status == SportMatchModelConst.TimeStatus.Ended) {
				last_matches.homes.Add(homeMatch);
			}
			else if (status == SportMatchModelConst.TimeStatus.Upcoming) {
				next_matches.homes.Add(homeMatch);
			}
		}

		// Build away's matches (last, next)
		for (var index = response.results.away.Count - 1; index >= 0; --index) {
			var item = response.results.away[index];
			var status = SportMatchModelConst.ConvertStatusFromBetsapi(item.time_status);

			var awayMatch = new Sport_GetMatchHistoryResponse.Away() {
				league = item.league.name,
				time = item.time,
				status = (int)status,
				scores = item.ss,
				home = new() {
					name = item.home.name,
					img = item.home.image_id
				},
				away = new() {
					name = item.away.name,
					img = item.away.image_id
				}
			};

			if (status == SportMatchModelConst.TimeStatus.Upcoming) {
				next_matches.aways.Add(awayMatch);
			}
			else if (status == SportMatchModelConst.TimeStatus.Ended) {
				last_matches.aways.Add(awayMatch);
			}
		}

		// Build last meetings of 2 teams
		var last_meetings = new Sport_GetMatchHistoryResponse.last_meetings();
		for (var index = response.results.home.Count - 1; index >= 0; --index) {
			var item = response.results.home[index];

			if (item.home.id == matchResult.ref_betsapi_home_team_id && item.away.id == matchResult.ref_betsapi_away_team_id) {
				last_meetings.list.Add(new() {
					league = item.league.name,
					scores = item.ss,
					time = item.time,
					home = new() {
						name = item.home.name,
						img = item.home.image_id,
					},
					away = new() {
						name = item.away.name,
						img = item.away.image_id
					},
				});
			}
		}

		// Summary
		var summary = new Sport_GetMatchHistoryResponse.Summary();
		var s1Max = int.MinValue;
		var s2WhenS1Max = int.MinValue;
		var s2Max = int.MinValue;
		var s1WhenS2Max = int.MinValue;
		foreach (var item in last_meetings.list) {
			var arr = item.scores.Split('-');
			if (arr.Length == 2) {
				var s1 = arr[0].ParseIntDk();
				var s2 = arr[1].ParseIntDk();

				if (s1Max < s1) {
					s1Max = s1;
					s2WhenS1Max = s2;
				}
				if (s2Max < s2) {
					s2Max = s2;
					s1WhenS2Max = s1;
				}

				summary.home.tt_goals += s1;
				summary.away.tt_goals += s2;
			}
		}
		var lastMeetingsCount = last_meetings.list.Count;
		if (lastMeetingsCount > 0) {
			summary.home.avg_goal_match = summary.home.tt_goals / lastMeetingsCount;
			summary.away.avg_goal_match = summary.away.tt_goals / lastMeetingsCount;
		}
		if (s1Max != int.MinValue) {
			summary.home.highest_win = $"{s1Max}:{s2WhenS1Max}";
		}
		if (s2Max != int.MinValue) {
			summary.away.highest_win = $"{s2Max}:{s1WhenS2Max}";
		}

		// Next match
		var next_meetings = new Sport_GetMatchHistoryResponse.Matches();

		return new Sport_GetMatchHistoryResponse {
			data = new() {
				summary = summary,
				next_meetings = next_meetings,
				last_meetings = last_meetings,
				last_matches = last_matches,
				next_matches = next_matches,
			}
		};
	}
}
