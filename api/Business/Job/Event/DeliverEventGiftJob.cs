namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class DeliverEventGiftJob : BaseJob<DeliverEventGiftJob> {
	private const string JOB_NAME = nameof(DeliverEventGiftJob);

	public static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<DeliverEventGiftJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.Now.AddSeconds(30))) // Delay
			.WithCronSchedule("10 /5 * * * ?") // Every 5 minutes
			.WithDescription(JOB_NAME)
		);
	}

	private readonly CardanoNodeRepo cardanoNodeRepo;
	private readonly SystemDao systemDao;

	public DeliverEventGiftJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<DeliverEventGiftJob> logger,
		MailComponent mailComponent,
		CardanoNodeRepo cardanoNodeRepo,
		SystemDao systemWalletDao
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger, mailComponent: mailComponent) {
		this.cardanoNodeRepo = cardanoNodeRepo;
		this.systemDao = systemWalletDao;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		var query =
			from _detail in this.dbContext.eventDetails

			join _event in this.dbContext.events on _detail.event_id equals _event.id
			join _user in this.dbContext.users on _detail.user_id equals _user.id

			where _detail.user_id != null
			where EventAirdropModelConst.NeedSubmitToChainStatusList.Contains(_detail.gift_delivery_tx_status)

			orderby _detail.updated_at ascending

			select new {
				email = _user.email,
				eventDetail = _detail,
				event_name = _event.name
			}
		;
		var results = await query.Take(100).ToArrayAsync();
		if (results.Length == 0) {
			return;
		}

		// Skip tx at Cardano if has few participants (< 10 applicants) in short elapsed time (30 minutes)
		if (results.Length < 10) {
			// Don't send tx if still short elapsed time
			var newerParticipant = results[0].eventDetail;
			var elapsedSeconds = DateTime.UtcNow.Subtract(newerParticipant.updated_at ?? newerParticipant.created_at).TotalSeconds;
			if (elapsedSeconds < 1800) {
				this.logger.InfoDk(this, "Skip deliver gift since still short elapsed time");
				return;
			}
		}

		// Perform send at Cardano
		var sysAddress = await this.systemDao.GetSystemGameAddressAsync();
		var abeAssetId = await this.systemDao.GetCardanoAssetIdAsync(MstCurrencyModelConst.NAME_ABE);
		var transactions = new List<CardanoNode_TxRawAssetsRequestBody.TransactionItem>();

		foreach (var item in results) {
			var eventDetail = item.eventDetail;

			// Send gift (ABE + ADA)
			transactions.Add(new() {
				sender_address = sysAddress,
				receiver_address = eventDetail.gift_receiver_address,
				assets = new CardanoNode_AssetInfo[] {
					new CardanoNode_AssetInfo {
						asset_id = MstCurrencyModelConst.CODE_ADA,
						quantity = $"{(eventDetail.gift_ada_amount * AppConst.ADA_COIN2TOKEN):0}"
					},
					new CardanoNode_AssetInfo {
						asset_id = abeAssetId,
						quantity = $"{((eventDetail.gift_abe_amount * AppConst.ABE_COIN2TOKEN)):0}"
					}
				}
			});
		}

		// Submit gift to chain
		var cnodeResponse = await this.cardanoNodeRepo.TxRawAssetsAsync(new() {
			fee_payer_addresses = new string[] { sysAddress },
			transactions = transactions.ToArray()
		});

		// Update delivery status
		var processCompletedSuccessfully = cnodeResponse.succeed;
		var giftDeliveryStatus = processCompletedSuccessfully ?
			EventAirdropModelConst.GiftDeliveryStatus.SubmitToChainSucceed :
			EventAirdropModelConst.GiftDeliveryStatus.SubmitToChainFailed
		;
		foreach (var item in results) {
			var eventDetail = item.eventDetail;

			eventDetail.gift_delivery_tx_status = giftDeliveryStatus;
			eventDetail.gift_delivery_tx_hash = cnodeResponse.data?.tx_id;
			eventDetail.gift_delivery_tx_result_message = cnodeResponse.message;
		}

		await this.dbContext.SaveChangesAsync();

		// Send mail after save changes
		if (processCompletedSuccessfully) {
			foreach (var item in results) {
				await this.mailComponent!.SendAsync(
					item.email,
					$"{item.event_name} event: Your gift was delivered successful",
					await MailTemplate.ForSentGiftOfEventAsync(item.event_name, item.eventDetail.gift_ada_amount, item.eventDetail.gift_abe_amount)
				);
			}
		}

		this.logger.InfoDk(this, $"End {JOB_NAME} for {results.Length} participants");
	}

	private async Task<string> GetRootUserWalletAdderssAsync() {
		var query =
			from _user in this.dbContext.users
			join _user_wallet in this.dbContext.userWallets on _user.id equals _user_wallet.user_id
			where _user.role == UserModelConst.Role.Root
			select new {
				wallet_address = _user_wallet.wallet_address
			}
		;

		// Find or throw if not found
		var result = await query.FirstAsync();

		return result.wallet_address;
	}
}
