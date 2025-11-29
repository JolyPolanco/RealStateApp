using AutoMapper;
using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class SaleTypeToDtoMappingProfile: Profile
    {
        public SaleTypeToDtoMappingProfile()
        {
            CreateMap<SaleType, SaleTypeDto>()
                .ForMember(dest => dest.PropertiesCount, opt => opt.MapFrom(src => src.Properties.Count()))
                .ReverseMap();
        }
    }
}
