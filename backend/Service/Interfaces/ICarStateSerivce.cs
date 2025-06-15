using SmartContractVehicle.DTO;

namespace SmartContractVehicle.Service
{
    public interface ICarStateService
    {
        Task<CarState?> GetAsync(string deviceId);
        Task UpdateAsync(CarState state);
    }

}
