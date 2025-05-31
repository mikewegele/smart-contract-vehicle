using AutoMapper;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Mapper
{
    public class CarApiMapper : Profile
    {
        public CarApiMapper()
        {
            CreateMap<Car, CarTO>();
        }
    }
}
