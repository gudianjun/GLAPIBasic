using AutoMapper;
using GLAPIBasic.Models;
using GLAPIBasic.Utilities;

namespace GLAPIBasic.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequest, UserInfo>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => StringHelper.HashPassword(src.Password)))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => System.DateTime.Now))
                .ForMember(dest => dest.AvatarThumbnail, opt => opt.MapFrom(src => src.AvatarThumbnail))
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
            // 转换到GetUserInfoResponse

            CreateMap<UserInfo, GetUserInfoResponse>() 
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.AvatarThumbnail, opt => opt.MapFrom(src => src.AvatarThumbnail));
        }
    }
}
