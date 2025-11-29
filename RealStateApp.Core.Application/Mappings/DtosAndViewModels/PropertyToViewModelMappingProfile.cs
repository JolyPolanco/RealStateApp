using AutoMapper;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.ViewModels.Properties;


namespace RealStateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class PropertyToViewModelMappingProfile : Profile
    {
        public PropertyToViewModelMappingProfile()
        {
            CreateMap<PropertyTypeDto, PropertyTypeViewModel>();
            CreateMap<PropertyTypeDto, SavePropertyTypeViewModel>();

        }
    }
}
