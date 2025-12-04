using AutoMapper;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.ViewModels.Properties;

namespace RealStateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class AgentPropertyMappingProfile : Profile
    {
        public AgentPropertyMappingProfile()
        {
            CreateMap<AgentPropertyDto, AgentPropertyViewModel>().ReverseMap();
            CreateMap<SavePropertyDto, SavePropertyViewModel>().ReverseMap();
        }
    }
}
