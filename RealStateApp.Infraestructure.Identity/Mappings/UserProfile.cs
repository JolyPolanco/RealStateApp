using AutoMapper;
using RealStateApp.Core.Application.Dtos.Login;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Infraestructure.Identity.Entities;

namespace RealStateApp.Infraestructure.Identity.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AppUser, LoginResponseDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore()) // Roles are mapped manually from UserManager
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.EmailConfirmed && src.IsActive))
                .ForMember(dest => dest.HasError, opt => opt.Ignore())
                .ForMember(dest => dest.Error, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? ""))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName ?? ""));

            CreateMap<AppUser, RegisterUserResponseDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore()) // Roles are mapped manually
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.EmailConfirmed))
                .ForMember(dest => dest.HasError, opt => opt.Ignore())
                .ForMember(dest => dest.Errors, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? ""))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName ?? ""))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Dni, opt => opt.MapFrom(src => src.Dni));
                
            CreateMap<AppUser, UserResponseDto>()
                .ForMember(dest => dest.HasError, opt => opt.Ignore())
                .ForMember(dest => dest.Errors, opt => opt.Ignore())
                .ForMember(dest => dest.Message, opt => opt.Ignore());
        }
    }
}
