namespace App;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public class UserCommand : BaseService {
	private readonly ILogger<UserCommand> logger;
	private readonly UserComponent userComponent;

	public UserCommand(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UserCommand> logger,
		UserComponent userComponent
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.userComponent = userComponent;
	}

	public async Task<ApiResponse> UpdateCodesForUsers() {
		var users = await this.dbContext.users.Where(m => m.code == null || m.referral_code == null).ToArrayAsync();

		var updateCount = 0;
		foreach (var user in users) {
			if (user.code is null) {
				user.code = UserComponent.CalcCodeFromSeed(user.no);
				updateCount++;
			}

			if (user.referral_code is null) {
				user.referral_code = UserComponent.CalcCodeFromSeed(user.no);
				updateCount++;
			}
		}

		if (updateCount > 0) {
			await this.dbContext.SaveChangesAsync();

			return new ApiSuccessResponse($"Updated {updateCount} users");
		}

		return new ApiSuccessResponse("Nothing to update");
	}
}
