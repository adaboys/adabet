namespace App;

using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tool.Compet.Core;
using Tool.Compet.EntityFrameworkCore;
using Tool.Compet.Json;

public class UserCoinTxService : BaseService {
	private readonly ILogger<UserCoinTxService> logger;

	public UserCoinTxService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<UserCoinTxService> logger
	) : base(dbContext, snapshot) {
		this.logger = logger;
	}

	public async Task<ApiResponse> GetCoinTxHistory(Guid user_id, int pagePos, int pageSize) {
		var query =
			from _tx in this.dbContext.coinTxs
			join _coin in this.dbContext.currencies on _tx.forward_currency_id equals _coin.id
			where _tx.seller_id == user_id || _tx.buyer_id == user_id
			select new GetCoinTxHistoryResponse.History {
				forward_currency_id = _tx.forward_currency_id
			}
		;
		var pagedResult = await query.AsNoTracking().PaginateDk(pagePos, pageSize);

		var histories = pagedResult.items;

		return new GetCoinTxHistoryResponse {
			data = new() {
				page_pos = pagedResult.pagePos,
				page_count = pagedResult.pageCount,
				total_item_count = pagedResult.totalItemCount,
				histories = histories
			}
		};
	}
}
