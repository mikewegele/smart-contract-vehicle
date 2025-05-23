using AutoMapper;
using SmartContractVehicle.DTO;
using SmartContractVehicle.Model;

namespace SmartContractVehicle.Mapper
{
    public class UserApiMapper : Profile
    {
        public UserApiMapper()
        {
            CreateMap<RegisterTO, User>();
            CreateMap<LoginTO, User>();
        }
    }
}
