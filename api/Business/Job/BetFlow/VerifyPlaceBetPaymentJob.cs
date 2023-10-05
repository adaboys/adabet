namespace App;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Tool.Compet.Core;
using Tool.Compet.Json;

[DisallowConcurrentExecution]
public class VerifyPlaceBetPaymentJob : BaseJob<VerifyPlaceBetPaymentJob> {
	private const string JOB_NAME = nameof(VerifyPlaceBetPaymentJob);

	internal static void Register(IServiceCollectionQuartzConfigurator quartzConfig) {
		quartzConfig.ScheduleJob<VerifyPlaceBetPaymentJob>(trigger => trigger
			.WithIdentity(JOB_NAME)
			.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(10))) // delay
			.WithCronSchedule("0 /1 * * * ?")
			.WithDescription(JOB_NAME)
		);
	}

	private readonly BlockfrostRepo blockfrostRepo;

	public VerifyPlaceBetPaymentJob(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<VerifyPlaceBetPaymentJob> logger,
		CardanoNodeRepo cardanoNodeRepo,
		BlockfrostRepo blockfrostRepo
	) : base(dbContext: dbContext, snapshot: snapshot, logger: logger) {
		this.blockfrostRepo = blockfrostRepo;
	}

	/// Override
	public override async Task Run(IJobExecutionContext context) {
		var query =
			from _ubet in this.dbContext.sportUserBets

			join _coin in this.dbContext.currencies on _ubet.bet_currency_id equals _coin.id

			where _ubet.submit_tx_status == SportUserBetModelConst.SubmitTxStatus.RequestVerifyPaymentAtChain

			select new {
				coin_unit = _coin.code.Replace(".", string.Empty),
				ubet = _ubet
			}
		;
		// Get all to sum-up on payment tx-hash of user's bets
		var result = await query.ToArrayAsync();
		if (result.Length == 0) {
			return;
		}

		// Sum up bet amount when user pay via external wallet.
		var txHash2totalBetAmount = new Dictionary<string, decimal>();
		foreach (var item in result) {
			var ubet = item.ubet;
			if (ubet.submit_tx_hash != null) {
				var betAmount = txHash2totalBetAmount.GetValueOrDefault(ubet.submit_tx_hash);
				txHash2totalBetAmount[ubet.submit_tx_hash] = betAmount + ubet.bet_currency_amount;
			}
		}

		// Check whether user paid at least total amount
		var saveChanges = false;
		foreach (var item in result) {
			var ubet = item.ubet;
			if (ubet.submit_tx_hash is null) {
				continue;
			}

			var utxoResponse = await this.blockfrostRepo.GetTransactionUtxosAsync(ubet.submit_tx_hash);
			if (utxoResponse is null || utxoResponse.inputs is null || utxoResponse.outputs is null) {
				continue;
			}

			var totalCoinInput = 0m;
			var totalCoinOutput = 0m;

			// Calculate total input on the coin
			foreach (var input in utxoResponse.inputs) {
				// Sum up coin-out from the user
				if (input.address == ubet.reward_receiver_address) {
					foreach (var amount in input.amount) {
						if (amount.unit == item.coin_unit) { // ADA unit is lovelace
							totalCoinInput += amount.quantity.ParseDecimalDk();
						}
					}
				}
			}

			// Calculate total output on the coin
			foreach (var output in utxoResponse.outputs) {
				// Sum up coin-in to game address
				if (output.address == ubet.reward_sender_address) {
					foreach (var amount in output.amount) {
						if (amount.unit == item.coin_unit) { // ADA unit is lovelace
							totalCoinOutput += amount.quantity.ParseDecimalDk();
						}
					}
				}
			}

			// Accepted payment since amount-of-user-paid and amount-of-we-got are >= bet-amount.
			saveChanges = true;
			if (txHash2totalBetAmount[ubet.submit_tx_hash] <= Math.Min(totalCoinInput, totalCoinOutput)) {
				ubet.submit_tx_status = SportUserBetModelConst.SubmitTxStatus.OnChainPersisted;
			}
			else {
				ubet.submit_tx_status = SportUserBetModelConst.SubmitTxStatus.InvalidPaymentAtChain;
			}
		}

		if (saveChanges) {
			await this.dbContext.SaveChangesAsync();
		}
	}
}
