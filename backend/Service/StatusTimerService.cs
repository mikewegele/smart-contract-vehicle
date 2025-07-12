using Microsoft.EntityFrameworkCore;
using SmartContractVehicle.Data;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Service
{
    /// <summary>
    /// This Service periodically (every 5 seconds) checks the database for Cars that are blocked or reserved and are over their timeout span
    /// The correct timespans are taken from Models.Reservation _reservationTime and _blockageTime
    /// </summary>
    public class StatusTimerService(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly TimeSpan _intervall = TimeSpan.FromSeconds(5);

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var statusPending = db.CarStatuses.Find((int)CarStatuses.Pending);
            if (statusPending is null) throw new NullReferenceException(nameof(statusPending));

            var statusReserved = db.CarStatuses.Find((int)CarStatuses.Reserved);
            if (statusReserved is null) throw new NullReferenceException(nameof(statusReserved));

            while (!ct.IsCancellationRequested)
            {
                var invalid = db.Cars
                    .Where(c =>
                        ((c.Status == statusPending && DateTime.UtcNow - c.LastStatusChange > Reservation._blockageTime) ||
                         (c.Status == statusReserved && DateTime.UtcNow - c.LastStatusChange > Reservation._reservationTime))
                        && c.ActiveReservation != null)
                    .ToListAsync(ct);

                (await invalid).ForEach(c => c.ActiveReservation?.CancleReservation(db));

                await db.SaveChangesAsync(CancellationToken.None); // we finish the last update before ending the loop

                await Task.Delay(_intervall, ct);
            }
        }

    }
}
