using AutoMapper;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class PropertyTypeToDtoMappingProfile : Profile
    {
        public PropertyTypeToDtoMappingProfile()
        {
            CreateMap<PropertyType, PropertyTypeDto>()
                .ForMember(dest => dest.PropertiesCount,
                    opt => opt.MapFrom(src => src.Properties != null ? src.Properties.Count : 0))
                .ReverseMap();
        }

    }
}
