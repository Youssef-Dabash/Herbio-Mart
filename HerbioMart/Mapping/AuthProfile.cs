using AutoMapper;
using HerbioMart.Models.Entities;
using HerbioMart.ViewModels.Auth;

namespace HerbioMart.Mapping;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<RegisterVM, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim().ToLower()))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.Trim()))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName.Trim()))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone.Trim()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}