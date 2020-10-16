using AutoMapper;
using DC = REST.API.DataContracts;
using S = REST.Services.Model;

namespace REST.IoC.Configuration.AutoMapper.Profiles
{
    public class APIMappingProfile : Profile
    {
        public APIMappingProfile()
        {
            CreateMap<DC.User, S.User>().ReverseMap();
            CreateMap<DC.Address, S.Address>().ReverseMap();
        }
    }
}
