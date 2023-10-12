namespace App;

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tool.Compet.Core;
using Tool.Compet.EntityFrameworkCore;
using Tool.Compet.Json;

public class SportService : BaseService {
	private BetsapiRepo betsapiRepo;

	public SportService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot) {
		this.betsapiRepo = betsapiRepo;
	}

	public async Task<ApiResponse> GetSportList() {
		// Get sport list
		var sports = await this.dbContext.sports.AsNoTracking()
			.Select(m => new Sport_ListResponse.Sport {
				id = m.id,
				name = m.name,
				_tmpOrder = m.order,
			})
			.OrderBy(m => m._tmpOrder)
			.ToArrayAsync()
		;
		var id2sport = sports.ToDictionary(m => m.id);

		// Because league list contains Dummy league (=1), we need query active sports
		// to get valid leagues.
		var sportIds = sports.Select(m => m.id).ToArray();
		var leagues = await this.dbContext.sportLeagues.AsNoTracking()
			.Where(m => sportIds.Contains(m.sport_id))
			.OrderByDescending(m => m.match_count)
			.Select(m => new Sport_ListResponse.League {
				id = m.id,
				name = m.name,
				_tmpSportId = m.sport_id,
			})
			.ToArrayAsync()
		;
		// Get at most 20 leagues on each sport
		foreach (var league in leagues) {
			var sportLeagues = id2sport[league._tmpSportId].leagues;
			if (sportLeagues.Count < 20) {
				sportLeagues.Add(league);
			}
		}

		return new Sport_ListResponse {
			data = new() {
				sports = sports
			}
		};
	}

	public async Task<ApiResponse> GetLiveMatches(Guid? user_id, int sport_id, int league_id) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			join _country in this.dbContext.countries on _league.country_id equals _country.id into _left_countries
			from _country in _left_countries.DefaultIfEmpty()

			where _league.sport_id == sport_id
			where league_id == 0 || _league.id == league_id
			where _match.status == SportMatchModelConst.TimeStatus.InPlay
			where _match.lock_mode == SportMatchModelConst.LockMode.Nothing

			orderby _match.start_at ascending

			select new Sport_MatchesResponse.Match {
				id = _match.id,
				sport_id = _league.sport_id,
				start_at = _match.start_at,
				status = (int)_match.status,

				country = _country.name,
				league = _league.name,

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

	public async Task<ApiResponse> GetUpcomingMatches(Guid? user_id, int sport_id, int league_id, int pagePos, int pageSize) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			join _country in this.dbContext.countries on _league.country_id equals _country.id into _left_countries
			from _country in _left_countries.DefaultIfEmpty()

			where _league.sport_id == sport_id
			where league_id == 0 || _league.id == league_id
			where _match.status == SportMatchModelConst.TimeStatus.Upcoming
			where _match.lock_mode == SportMatchModelConst.LockMode.Nothing
			where _match.markets != null // Ignore matches that does not ready for betting

			orderby _match.start_at ascending

			select new Sport_UpcomingMatchesResponse.Match {
				id = _match.id,
				sport_id = _league.sport_id,
				start_at = _match.start_at,
				status = (int)_match.status,

				country = _country.name,
				league = _league.name,

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

	public async Task<ApiResponse> GetHighlightMatches(int sport_id, int league_id) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			join _country in this.dbContext.countries on _league.country_id equals _country.id into _left_countries
			from _country in _left_countries.DefaultIfEmpty()

			where ((int)_league.sport_id) == sport_id
			where league_id == 0 || _league.id == league_id
			where _match.status == SportMatchModelConst.TimeStatus.InPlay
			where _match.lock_mode == SportMatchModelConst.LockMode.Nothing

			orderby _match.user_bet_count descending, _match.start_at ascending

			select new Sport_GetHighlightMatchesResponse.Match {
				id = _match.id,
				sport_id = _league.sport_id,
				start_at = _match.start_at,
				status = (int)_match.status,

				country = _country.name,
				league = _league.name,

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

	public async Task<ApiResponse> GetTopMatches(Guid? user_id, int sport_id, int league_id) {
		var query =
			from _match in this.dbContext.sportMatches

			join _league in this.dbContext.sportLeagues on _match.league_id equals _league.id
			join _team1 in this.dbContext.sportTeams on _match.home_team_id equals _team1.id
			join _team2 in this.dbContext.sportTeams on _match.away_team_id equals _team2.id
			join _country in this.dbContext.countries on _league.country_id equals _country.id into _left_countries
			from _country in _left_countries.DefaultIfEmpty()

			where _league.sport_id == sport_id
			where league_id == 0 || _league.id == league_id
			where _match.status == SportMatchModelConst.TimeStatus.InPlay
			where _match.lock_mode == SportMatchModelConst.LockMode.Nothing

			orderby _match.start_at ascending

			select new Sport_GetTopMatchesResponse.Match {
				id = _match.id,
				sport_id = _league.sport_id,
				start_at = _match.start_at,
				status = (int)_match.status,

				country = _country.name,
				league = _league.name,

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

		var lastMatches = new Sport_GetMatchHistoryResponse.Matches();
		var nextMatches = new Sport_GetMatchHistoryResponse.Matches();

		// Build home's last_matches and next_matches
		for (var index = response.results.home.Count - 1; index >= 0; --index) {
			var apiItem = response.results.home[index];
			var status = SportMatchModelConst.ConvertStatusFromBetsapi(apiItem.time_status.ParseIntDk());

			var homeMatch = new Sport_GetMatchHistoryResponse.Home() {
				league = apiItem.league.name,
				time = apiItem.time,
				status = (int)status,
				scores = apiItem.ss,
				home = new() {
					name = apiItem.home.name,
					img = apiItem.home.image_id
				},
				away = new() {
					name = apiItem.away.name,
					img = apiItem.away.image_id
				}
			};

			if (status == SportMatchModelConst.TimeStatus.Ended) {
				lastMatches.homes.Add(homeMatch);
			}
			else if (status == SportMatchModelConst.TimeStatus.Upcoming) {
				nextMatches.homes.Add(homeMatch);
			}
		}

		// Build away's last_matches and next_matches
		for (var index = response.results.away.Count - 1; index >= 0; --index) {
			var apiItem = response.results.away[index];
			var status = SportMatchModelConst.ConvertStatusFromBetsapi(apiItem.time_status.ParseIntDk());

			var awayMatch = new Sport_GetMatchHistoryResponse.Away() {
				league = apiItem.league.name,
				time = apiItem.time,
				status = (int)status,
				scores = apiItem.ss,
				home = new() {
					name = apiItem.home.name,
					img = apiItem.home.image_id
				},
				away = new() {
					name = apiItem.away.name,
					img = apiItem.away.image_id
				}
			};

			if (status == SportMatchModelConst.TimeStatus.Upcoming) {
				nextMatches.aways.Add(awayMatch);
			}
			else if (status == SportMatchModelConst.TimeStatus.Ended) {
				lastMatches.aways.Add(awayMatch);
			}
		}

		// Build last_meetings of 2 teams
		var lastMeetings = new Sport_GetMatchHistoryResponse.LastMeeting();
		var betsapi_homeTeamId = matchResult.ref_betsapi_home_team_id;
		var betsapi_awayTeamId = matchResult.ref_betsapi_away_team_id;
		for (var index = response.results.home.Count - 1; index >= 0; --index) {
			var apiItem = response.results.home[index];

			var isLastMeeting =
				(apiItem.home.id == betsapi_homeTeamId && apiItem.away.id == betsapi_awayTeamId) ||
				(this.appSetting.environment != AppSetting.ENV_PRODUCTION)
			;
			if (isLastMeeting) {
				lastMeetings.list.Add(new() {
					league = apiItem.league.name,
					scores = apiItem.ss,
					time = apiItem.time,
					home = new() {
						name = apiItem.home.name,
						img = apiItem.home.image_id,
					},
					away = new() {
						name = apiItem.away.name,
						img = apiItem.away.image_id
					},
				});
			}
		}

		// Build next_meetings of 2 teams
		var nextMeetings = new Sport_GetMatchHistoryResponse.NextMeeting();
		for (var index = response.results.home.Count - 1; index >= 0; --index) {
			var apiItem = response.results.home[index];
			var timeStatus = SportMatchModelConst.ConvertStatusFromBetsapi(apiItem.time_status.ParseIntDk());

			var isNextMeeting =
				(apiItem.home.id == betsapi_homeTeamId && apiItem.away.id == betsapi_awayTeamId && timeStatus == SportMatchModelConst.TimeStatus.Upcoming) ||
				(this.appSetting.environment != AppSetting.ENV_PRODUCTION)
			;
			if (isNextMeeting) {
				nextMeetings.list.Add(new() {
					league = apiItem.league.name,
					scores = apiItem.ss,
					time = apiItem.time,
					home = new() {
						name = apiItem.home.name,
						img = apiItem.home.image_id,
					},
					away = new() {
						name = apiItem.away.name,
						img = apiItem.away.image_id
					},
				});
			}
		}

		// Summary
		// - Total goals
		// - Victories
		var summary = new Sport_GetMatchHistoryResponse.Summary();
		var s1Max = int.MinValue;
		var s2WhenS1Max = int.MinValue;
		var s2Max = int.MinValue;
		var s1WhenS2Max = int.MinValue;
		foreach (var item in lastMeetings.list) {
			var arr = item.scores.Split('-');
			// Soccer
			if (arr.Length == 2) {
				var s1 = arr[0].ParseIntDk();
				var s2 = arr[1].ParseIntDk();

				if (s1 > s2) {
					++summary.home.victories;
				}
				else if (s1 < s2) {
					++summary.away.victories;
				}
				else {
					++summary.draw;
				}

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

		// - Performance
		foreach (var item in lastMatches.homes) {
			var scores = item.scores.Split('-');
			// Soccer
			if (scores.Length == 2) {
				summary.home.performance.Add(new() {
					s1 = scores[0].ParseIntDk(),
					s2 = scores[1].ParseIntDk(),
					time = item.time,
				});
			}
		}
		foreach (var item in lastMatches.aways) {
			var scores = item.scores.Split('-');
			// Soccer
			if (scores.Length == 2) {
				summary.home.performance.Add(new() {
					s1 = scores[0].ParseIntDk(),
					s2 = scores[1].ParseIntDk(),
					time = item.time,
				});
			}
		}

		// - Avg goal
		var lastMeetingsCount = lastMeetings.list.Count;
		if (lastMeetingsCount > 0) {
			summary.home.avg_goal_match = summary.home.tt_goals / lastMeetingsCount;
			summary.away.avg_goal_match = summary.away.tt_goals / lastMeetingsCount;
		}

		// - Highest win
		if (s1Max != int.MinValue) {
			summary.home.highest_win = $"{s1Max}:{s2WhenS1Max}";
		}
		if (s2Max != int.MinValue) {
			summary.away.highest_win = $"{s2Max}:{s1WhenS2Max}";
		}

		return new Sport_GetMatchHistoryResponse {
			data = new() {
				summary = summary,
				next_meetings = nextMeetings,
				last_meetings = lastMeetings,
				last_matches = lastMatches,
				next_matches = nextMatches,
			}
		};
	}
}
