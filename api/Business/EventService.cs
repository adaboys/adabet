namespace App;

using System;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tool.Compet.EntityFrameworkCore;

public class EventService : BaseService {
	private readonly ILogger<EventService> logger;
	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly UserDao userDao;

	public EventService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<EventService> logger,
		CardanoNodeRepo cardanoNodeRepo,
		UserDao userDao
	) : base(dbContext: dbContext, snapshot: snapshot) {
		this.logger = logger;
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.userDao = userDao;
	}

	public async Task<ApiResponse> RegisterEvent(Guid userId, string event_name) {
		var eventInfo = await this.dbContext.events.FirstOrDefaultAsync(m =>
			m.name == event_name &&
			m.visibility == EventModelConst.Visibility.Public &&
			m.status == EventModelConst.Status.Normal
		);
		if (eventInfo is null) {
			return new ApiBadRequestResponse("No event");
		}

		var query_user =
			from _user in this.dbContext.users

			join _wallet in this.dbContext.userWallets on _user.id equals _wallet.user_id

			where _user.id == userId && _user.status == UserModelConst.Status.Normal && _user.deleted_at == null
			where _wallet.wallet_type == UserWalletModelConst.WalletType.Internal
			where _wallet.wallet_status == UserWalletModelConst.WalletStatus.Active

			select new {
				wallet_address = _wallet.wallet_address
			}
		;
		var result_user = await query_user.FirstOrDefaultAsync();
		if (result_user is null) {
			return new ApiBadRequestResponse("Invalid user");
		}

		var participant = await this.dbContext.eventDetails.FirstOrDefaultAsync(m => m.user_id == userId);
		if (participant != null) {
			return new ApiBadRequestResponse("Already joined") { code = ErrCode.already_registered };
		}

		// Commented: since for now, we use internal wallet.
		// Validate receiver address
		// var gift_receiver_address = requestBody.gift_receive_address;
		// if (!CardanoHelper.IsAddressMatchWithEnv(this.appSetting, gift_receiver_address)) {
		// 	return new ApiBadRequestResponse() { code = ErrCode.address_not_match_env };
		// }
		// var addressResponse = await this.cardanoNodeRepo.ValidateAddressAsync(gift_receiver_address);
		// if (addressResponse.failed) {
		// 	return addressResponse;
		// }

		// By default, we give to each user 2 ADA and 50 ABE.
		// For each event, it maybe changed.
		var giftReceiverAddress = result_user.wallet_address;
		var giftAdaAmount = 2m;
		var giftAbeAmount = 50m;

		// Start transaction
		using (var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
			// Lock on the event
			eventInfo = await this.dbContext.events
				.FromSqlRaw($"SELECT * FROM {DbConst.table_event} WITH (UPDLOCK) WHERE id = {{0}}", eventInfo.id)
				.FirstAsync()
			;
			if (eventInfo.visibility != EventModelConst.Visibility.Public || eventInfo.status != EventModelConst.Status.Normal) {
				return new ApiInternalServerErrorResponse("Invalid event");
			}
			if (eventInfo.current_applicant_count >= eventInfo.max_applicant_count) {
				return new ApiBadRequestResponse("Event room full") { code = ErrCode.room_full };
			}

			// Each user can join at most once
			if (await this.dbContext.eventDetails.AnyAsync(m => m.user_id == userId)) {
				return new ApiBadRequestResponse("Already participated") { code = ErrCode.already_registered };
			}

			this.dbContext.eventDetails.Attach(new() {
				event_id = eventInfo.id,
				user_id = userId,
				gift_receiver_address = giftReceiverAddress,
				gift_ada_amount = giftAdaAmount,
				gift_abe_amount = giftAbeAmount,
				gift_delivery_tx_status = EventAirdropModelConst.GiftDeliveryStatus.RequestSubmitToChain,
			});

			// Consume one seat
			++eventInfo.current_applicant_count;

			await this.dbContext.SaveChangesAsync();
			txScope.Complete();

			return new ApiSuccessResponse();
		}
	}

	public async Task<ApiResponse> GetEventApplicants(int pagePos, int pageSize) {
		var query =
			from _detail in this.dbContext.eventDetails

			join _user in this.dbContext.users on _detail.user_id equals _user.id

			where _detail.user_id != null

			orderby _detail.updated_at descending, _detail.created_at descending

			select new {
				player_name = _user.player_name,
				gift_ada_amount = _detail.gift_ada_amount,
				gift_abe_amount = _detail.gift_abe_amount,
				gift_receive_status = _detail.gift_delivery_tx_status,
			}
		;
		var pagedResult = await query.PaginateDk(pagePos, pageSize);
		var items = pagedResult.items;

		var data_participants = new EventResponse.Applicant[items.Length];
		for (var index = items.Length - 1; index >= 0; --index) {
			var item = items[index];

			data_participants[index] = new() {
				player_name = item.player_name,
				gift_ada_amount = item.gift_ada_amount,
				gift_abe_amount = item.gift_abe_amount,
				gift_delivery_status = ((byte)item.gift_receive_status),
			};
		}

		return new EventResponse {
			data = new() {
				page_pos = pagedResult.pagePos,
				page_count = pagedResult.pageCount,
				total_item_count = pagedResult.totalItemCount,
				applicants = data_participants
			}
		};
	}
}
