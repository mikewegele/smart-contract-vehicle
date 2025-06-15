using SmartContractVehicle.DTO;
using System.Collections.Concurrent;

namespace SmartContractVehicle.Service
{

    public class InMemoryCarStateService : ICarStateService
    {
        private readonly ConcurrentDictionary<string, CarState> _store = new();

        public Task<CarState?> GetAsync(string deviceId)
        {
            _store.TryGetValue(deviceId, out var state);
            return Task.FromResult(state);
        }

        public Task UpdateAsync(CarState state)
        {
            state.LastUpdated = DateTime.UtcNow;
            _store[state.DeviceId] = state;
            return Task.CompletedTask;
        }
    }

}
