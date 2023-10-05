namespace App;

using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tool.Compet.Core;

public class CurrencyService : BaseService {
	private readonly ILogger<CurrencyService> logger;
	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly UserDao userDao;

	public CurrencyService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<CurrencyService> logger,
		CardanoNodeRepo cardanoNodeRepo,
		UserDao userDao
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.userDao = userDao;
	}

	public async Task<ApiResponse> GetCurrencyList(Guid? userId) {
		var currencies = await this.dbContext.currencies
			.Select(m => new GetCurrencyListResponse.Currency {
				tmpDecimal = m.decimals,
				tmpCode = m.code,

				id = m.id,
				name = m.name,
			})
			.ToArrayAsync()
		;

		if (userId != null) {
			var userWalletAddr = await this.userDao.GetUserInternalWalletOrThrowAsync(userId.Value);
			var balanceResponse = await this.cardanoNodeRepo.GetMergedAssetsAsync(userWalletAddr);
			if (balanceResponse.failed) {
				return balanceResponse;
			}

			var code2asset = balanceResponse.data.assets.ToDictionary(m => m.asset_id);
			foreach (var currency in currencies) {
				var amount = code2asset.GetValueOrDefault(currency.tmpCode)?.quantity.ParseDecimalDk() ?? 0m;
				currency.amount = amount / DkMaths.Pow(10, currency.tmpDecimal);
			}
		}

		return new GetCurrencyListResponse {
			data = new() {
				currencies = currencies
			}
		};
	}
}
