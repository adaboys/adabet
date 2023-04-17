namespace App;

using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tool.Compet.Core;

public class CurrencyService : BaseService {
	private readonly ILogger<CurrencyService> logger;
	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly UserWalletDao userWalletDao;

	public CurrencyService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<CurrencyService> logger,
		CardanoNodeRepo cardanoNodeRepo,
		UserWalletDao userWalletDao
	) : base(dbContext, snapshot) {
		this.logger = logger;
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.userWalletDao = userWalletDao;
	}

	public async Task<ApiResponse> GetUserCurrencyList(Guid userId) {
		var currencies = await this.dbContext.currencies
			.Select(m => new GetCurrencyListResponse.Currency {
				id = m.id,
				name = m.name,
				tmpDecimal = m.decimals,
				tmpCode = m.code,
			})
			.ToArrayAsync()
		;

		var userWalletAddr = await this.userWalletDao.GetUserInternalWalletOrThrowAsync(userId);
		var balanceResponse = await this.cardanoNodeRepo.GetMergedAssetsAsync(userWalletAddr);
		if (balanceResponse.failed) {
			return balanceResponse;
		}

		var code2asset = balanceResponse.data.assets.ToDictionary(m => m.asset_id);
		foreach (var currency in currencies) {
			var amount = code2asset.GetValueOrDefault(currency.tmpCode)?.quantity.ParseDecimalDk() ?? 0m;
			currency.amount = amount / DkMaths.Pow(10, currency.tmpDecimal);
		}

		return new GetCurrencyListResponse {
			data = new() {
				currencies = currencies
			}
		};
	}
}
