using System;
using System.Threading.Tasks;

namespace SmartContractVehicle.Services
{
    public class CarStatusService
    {
        public async Task<bool> IsCarReadyToEndRideAsync(Guid carId)
        {
            // TODO: Replace with real logic to check if the car is stationary and empty.
            return await Task.FromResult(true);
        }

        public async Task<bool> TryLockCarAsync(Guid carId)
        {
            // TODO: Replace with real logic to send a lock command to the car.
            return await Task.FromResult(true);
        }
    }
}