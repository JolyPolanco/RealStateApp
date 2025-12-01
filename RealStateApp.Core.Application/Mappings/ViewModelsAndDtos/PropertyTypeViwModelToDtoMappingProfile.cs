using AutoMapper;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.ViewModels.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Mappings.ViewModelsAndDtos
{
    public class PropertyTypeViwModelToDtoMappingProfile : Profile
    {
        public PropertyTypeViwModelToDtoMappingProfile()
        {
            CreateMap<SavePropertyTypeViewModel, PropertyTypeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? 0))
                .ReverseMap();
        }
    }
}
