using Microsoft.EntityFrameworkCore;
using SmartContractVehicle.Data;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Service
{
    /// <summary>
    /// This Service periodically (every 5 seconds) checks the database for Cars that are blocked or reserved and are over their timeout span (reserved for longer than 15 Minutes or blocked for more than 30 seconds)
    /// </summary>
    public class StatusTimerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _intervall = TimeSpan.FromSeconds(5);

        public StatusTimerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var statusAvailable = db.CarStatuses.Find((int)CarStatuses.Available);
            if (statusAvailable is null) throw new NullReferenceException(nameof(statusAvailable));

            var statusPending = db.CarStatuses.Find((int)CarStatuses.Pending);
            if (statusPending is null) throw new NullReferenceException(nameof(statusPending));

            var statusReserved = db.CarStatuses.Find((int)CarStatuses.Reserved);
            if (statusReserved is null) throw new NullReferenceException(nameof(statusReserved));

            while (!ct.IsCancellationRequested)
            {
                Task invalid =  db.Cars
                    .Where(c => 
                        (c.Status == statusPending && DateTime.UtcNow.Subtract(c.LastStatusChange.Value) > Reservation._blockageTime ) ||
                        (c.Status == statusReserved && DateTime.UtcNow.Subtract(c.LastStatusChange.Value) > Reservation._reservationTime )
                        )
                    .ForEachAsync(o => o.SetStatus(statusAvailable), CancellationToken.None);

                await invalid;
                await db.SaveChangesAsync(CancellationToken.None); // we finish the last update before ending the loop

                await Task.Delay(_intervall, ct);
            }
        }

    }
}
