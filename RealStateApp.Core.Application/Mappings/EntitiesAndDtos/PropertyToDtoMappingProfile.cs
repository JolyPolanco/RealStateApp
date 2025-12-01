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
    public class PropertyToDtoMappingProfile : Profile
    {
        public PropertyToDtoMappingProfile()
        {
            CreateMap<Property, PropertyDto>();




            CreateMap<Property, DataPropertyDto>()
                .ForMember(s => s.Photo, opt => opt.Ignore())
                .ForMember(s => s.SaleType, opt => opt.Ignore())
                .ForMember(s => s.TypeProperty, opt => opt.Ignore());

            CreateMap<Property, DetailsPropertyDto>()
                .ForMember(s => s.Images, opt => opt.Ignore())
                .ForMember(s => s.Name, opt => opt.Ignore())
                .ForMember(s => s.Email, opt => opt.Ignore())
                .ForMember(s => s.UrlImage, opt => opt.Ignore())
                .ForMember(s => s.Inproments, opt => opt.Ignore())
                .ForMember(s => s.PhoneNumber, opt => opt.Ignore())
                .ForMember(s => s.SaleType, opt => opt.Ignore())
                .ForMember(s => s.PropertyType, opt => opt.Ignore());

        }
    }
}
