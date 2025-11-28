using AutoMapper;
using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Application.ViewModels.SaleType;


namespace RealStateApp.Core.Application.Mappings.ViewModelsAndDtos
{
    public class SaleTypeToDtoMappingProfile : Profile
    {
        public SaleTypeToDtoMappingProfile()
        {
            CreateMap<SaleTypeViewModel, SaleTypeDto>();
        }
    }
}
