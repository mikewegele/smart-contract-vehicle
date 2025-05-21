using AutoMapper;
using User.Transport;
using User.Domain;

namespace User.Api.Mapper
{
    public class UserApiMapper : Profile
    {
        public UserApiMapper()
        {
            CreateMap<NewUserTO, NewUser>();
        }
    }
}
