namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class Betsapi_FetchUpcomingMatchesJob : BaseJob {
	private const string JOB_NAME = nameof(Betsapi_FetchUpcomingMatchesJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<Betsapi_FetchUpcomingMatchesJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 0 /6 * * ?") // Run at everyday
			.WithDescription(JOB_NAME)
		);
	}

	private readonly ILogger<Betsapi_FetchUpcomingMatchesJob> logger;
	private readonly BetsapiRepo betsapiRepo;

	public Betsapi_FetchUpcomingMatchesJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<Betsapi_FetchUpcomingMatchesJob> logger,
		BetsapiRepo betsapiRepo
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.betsapiRepo = betsapiRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		var now = DateTime.UtcNow;
		var moreDay = 3;

		// Fetch upcoming matches
		while (moreDay-- > 0) {
			// Padding 0: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
			var day = $"{(now.Year):D4}{(now.Month):D2}{(now.Day):D2}";

			await this._RecursiveFetchUpcomingMatches(day, page: 1);

			now = now.AddDays(1);
		}
	}

	private async Task _RecursiveFetchUpcomingMatches(string day, int page) {
		// Focus on football
		var sport_id = await this.dbContext.sports.Where(m => m.name == MstSportModelConst.Name_Football).Select(m => m.id).FirstAsync();

		var apiResult = await this.betsapiRepo.FetchUpcomingMatches(sport_id, day, page);
		if (apiResult is null) {
			return;
		}

		var apiMatches = apiResult.results;

		foreach (var apiMatch in apiMatches) {
			var sysMatch = await this.dbContext.sportMatches.FirstOrDefaultAsync(m =>
				m.ref_betsapi_match_id == apiMatch.id
			);

			// Register new match with its info (league, team,...)
			if (sysMatch is null) {
				sysMatch = await this._RegisterNewMatch(sport_id, apiMatch);
			}
		}

		// Save all matches
		await this.dbContext.SaveChangesAsync();

		// Continuous fetch until reach to final page !
		if (apiResult.pager.page * apiResult.pager.per_page < apiResult.pager.total) {
			await this._RecursiveFetchUpcomingMatches(day, apiResult.pager.page + 1);
		}
	}

	private async Task<SportMatchModel> _RegisterNewMatch(int sport_id, Betsapi_UpcomingMatchesData.Result apiMatch) {
		var targetLeague = await this.dbContext.sportLeagues.FirstOrDefaultAsync(m =>
			m.ref_betsapi_league_id == apiMatch.league.id
		);
		var homeTeam = await this.dbContext.sportTeams.FirstOrDefaultAsync(m =>
			m.ref_betsapi_home_team_id == apiMatch.home.id
		);
		var awayTeam = await this.dbContext.sportTeams.FirstOrDefaultAsync(m =>
			m.ref_betsapi_away_team_id == apiMatch.away.id
		);

		// Save league and Get id
		if (targetLeague is null) {
			targetLeague = new() {
				sport_id = sport_id,
				name = apiMatch.league.name
			};
			this.dbContext.sportLeagues.Attach(targetLeague);
			await this.dbContext.SaveChangesAsync();
		}

		// Save home team and Get id
		if (homeTeam is null) {
			var image_id = await this.betsapiRepo.CalcImageId(apiMatch.home.image_id);

			homeTeam = new() {
				name = apiMatch.home.name,
				flag_image_name = image_id,
				flag_image_src = SportTeamModelConst.FlagImageSource.Betsapi,
				ref_betsapi_home_team_id = apiMatch.home.id
			};
			this.dbContext.sportTeams.Attach(homeTeam);
			await this.dbContext.SaveChangesAsync();
		}

		// Save away team and Get id
		if (awayTeam is null) {
			var image_id = await this.betsapiRepo.CalcImageId(apiMatch.away.image_id);

			awayTeam = new() {
				name = apiMatch.away.name,
				flag_image_name = image_id,
				flag_image_src = SportTeamModelConst.FlagImageSource.Betsapi,
				ref_betsapi_away_team_id = apiMatch.away.id
			};
			this.dbContext.sportTeams.Attach(awayTeam);
			await this.dbContext.SaveChangesAsync();
		}

		// Attach new match
		var targetMatch = new SportMatchModel() {
			league_id = targetLeague.id,
			home_team_id = homeTeam.id,
			away_team_id = awayTeam.id,

			status = SportMatchModelConst.TimeStatus.Upcoming,
			start_at = DateTimeOffset.FromUnixTimeSeconds(apiMatch.time).UtcDateTime,

			ref_betsapi_match_id = apiMatch.id,
			ref_betsapi_home_team_id = apiMatch.home.id,
			ref_betsapi_away_team_id = apiMatch.away.id,
		};

		this.dbContext.sportMatches.Attach(targetMatch);

		return targetMatch;
	}
}
