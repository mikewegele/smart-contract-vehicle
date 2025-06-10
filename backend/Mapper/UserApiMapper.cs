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
            CreateMap<User, UserTO>();
            CreateMap<UserProfileUpdateTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}
