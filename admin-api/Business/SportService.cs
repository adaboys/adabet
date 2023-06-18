namespace App;

using System;
using System.Text;
using System.Transactions;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RedLockNet;
using Tool.Compet.Core;
using Tool.Compet.EntityFrameworkCore;
using Tool.Compet.Json;

/// Raw query with interpolated: https://docs.microsoft.com/en-us/ef/core/querying/raw-sql
public class SportService : BaseService {
	private readonly ILogger<SportService> logger;

	public SportService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<SportService> logger
	) : base(dbContext, snapshot) {
		this.logger = logger;
	}

	public async Task<ApiResponse> SearchMatches(
		int sport_id,
		string? team_name,
		string? match_ids,
		int pagePos,
		int pageSize,
		string? sortType
	) {
		var matchIds = match_ids is null ? null : match_ids.Split(',').Select(m => m.ParseLongDk()).ToArray();

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

			// Search like team
			where team_name == null || _team1.name.Contains(team_name) || _team2.name.Contains(team_name)

			// Search match_ids
			where matchIds == null || matchIds.Contains(_match.id)

			//orderby _match.predictable descending, _match.start_at ascending

			select new Sport_UpcomingMatchesResponse.Match {
				id = _match.id,
				start_at = _match.start_at,
				created_at = _match.created_at,

				country = _country.name,
				league = _league.name,

				team1 = _team1.name,
				team2 = _team2.name,

				predictable = _match.predictable,

				image1 = SportTeamModelConst.CalcFlagImageName(_team1.flag_image_name, _team1.flag_image_src),
				image2 = SportTeamModelConst.CalcFlagImageName(_team2.flag_image_name, _team2.flag_image_src),

				score1 = _match.home_score,
				score2 = _match.away_score,

				timer = _match.timer,
				// markets = _match.markets == null ? null : DkJsons.ToObj<List<Market>>(_match.markets!),
			}
		;


		switch (sortType) {
			case "start_at_asc": {
				query = query
					.OrderBy(m => m.start_at)
					.ThenByDescending(m => m.predictable)
				;
				break;
			}
			case "start_at_desc": {
				query = query
					.OrderByDescending(m => m.start_at)
					.ThenByDescending(m => m.predictable)
				;
				break;
			}
			case "created_at_asc": {
				query = query
					.OrderBy(m => m.created_at)
					.ThenByDescending(m => m.predictable)
					.ThenBy(m => m.start_at)
				;
				break;
			}
			case "created_at_desc": {
				query = query
					.OrderByDescending(m => m.created_at)
					.ThenByDescending(m => m.predictable)
					.ThenBy(m => m.start_at)
				;
				break;
			}
			default: {
				query = query
					.OrderByDescending(m => m.predictable)
					.ThenBy(m => m.start_at)
				;
				break;
			}
		}

		var pagedResult = await query.AsNoTracking().PaginateDk(pagePos, pageSize);
		var matches = pagedResult.items;

		return new Sport_UpcomingMatchesResponse {
			data = new() {
				page_pos = pagedResult.pagePos,
				page_count = pagedResult.pageCount,
				total_item_count = pagedResult.totalItemCount,
				matches = matches
			}
		};
	}

	public async Task<ApiResponse> UpdateMatch(long match_id, UpdateMatchRequestBody req) {
		var match = await this.dbContext.sportMatches.FirstOrDefaultAsync(m => m.id == match_id);
		if (match is null) {
			return new ApiBadRequestResponse("No match");
		}

		var actions = new StringBuilder();

		if (req.predictable != null) {
			match.predictable = (bool)req.predictable;
			actions.Append("[Predictable: ").Append(match.predictable).Append("] ");
		}

		if (actions.Length > 0) {
			await this.dbContext.SaveChangesAsync();
		}

		return new ApiSuccessResponse(actions.ToString());
	}
}
