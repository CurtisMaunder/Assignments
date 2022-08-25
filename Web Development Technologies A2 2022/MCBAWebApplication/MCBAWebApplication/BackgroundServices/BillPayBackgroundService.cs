using Microsoft.EntityFrameworkCore;
using MCBAWebApplication.Data;
using MCBAWebApplication.Models;

namespace MCBAWebApplication.BackgroundServices;
public class BillPayBackgroundService : BackgroundService {
    private readonly IServiceProvider _serviceProvider;

    public BillPayBackgroundService(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        while (!cancellationToken.IsCancellationRequested) {
            await ProcessPayment(cancellationToken);

            await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
        }
    }

    private async Task ProcessPayment(CancellationToken cancellationToken) {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<McbaWebContext>();

        var now = DateTime.UtcNow;

        //See if it can be made so that it checks the hour as opposed to a utc now
        //Or perhaps up the frequency so that it doesnt really matter
        var scheduledPayments = await context.BillPays.Where(x => x.ScheduleTimeUtc <= now.ToUniversalTime() && x.Status == Status.Awaiting).ToListAsync(cancellationToken);
        foreach (var scheduledPayment in scheduledPayments) {
            //PROCESS
            /* Ignore anything that isnt marked as "awaiting
             * If the payment is monthly, process it and keep as awaiting if success or failed if failed
             * when application starts only look for awaiting
             */

            //DECOUPLE ALL THIS LATER VERY NAUGHTY
            //Check if sufficient funds then update balance
            var account = await context.Accounts.FindAsync(scheduledPayment.AccountNumber);
            
            if (account.Balance < scheduledPayment.Amount) {
                scheduledPayment.Status = Status.Failure;
                continue;
            }

            account.Balance -= scheduledPayment.Amount;

            if (scheduledPayment.Period == Period.Monthly)
                scheduledPayment.ScheduleTimeUtc = scheduledPayment.ScheduleTimeUtc.AddMonths(1);
            else if (scheduledPayment.Period == Period.OneOff)
                scheduledPayment.Status = Status.Success;

            await context.SaveChangesAsync();

        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
