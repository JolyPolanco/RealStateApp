using AutoMapper;
using RealStateApp.Core.Application.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserDto, AgentDto>()
                .ForMember(dest => dest.PropertiesCount, opt => opt.MapFrom(src => 0));
        }
    }
}
