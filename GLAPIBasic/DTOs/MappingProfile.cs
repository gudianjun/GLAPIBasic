using AutoMapper;
using GLAPIBasic.Models;
using GLAPIBasic.Utilities;

namespace GLAPIBasic.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequest, User>()
             .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.Password, opt => opt.MapFrom(src => StringHelper.HashPassword(src.Password)))
                ;
        }
    }
}
