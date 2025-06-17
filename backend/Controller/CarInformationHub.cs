using Microsoft.AspNetCore.SignalR;
using SmartContractVehicle.DTO;
using System.Collections.Concurrent;

namespace SmartContractVehicle.Controller
{
    public class CarInformationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, CarState> CarStates = new();

        public async Task UpdateState(CarState cs)
        {
            throw new NotImplementedException();
        }

        public async Task RequestSync(string CarId)
        {
            throw new NotImplementedException();
        }
    }
}
