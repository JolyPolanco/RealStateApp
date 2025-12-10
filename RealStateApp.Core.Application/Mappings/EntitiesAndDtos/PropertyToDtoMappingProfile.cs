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
            CreateMap<PropertyDto, Property>()
                .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                .ForMember(dest => dest.SaleType, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyImprovements, opt => opt.Ignore())
                .ForMember(dest => dest.Photos, opt => opt.Ignore())
                .ForMember(dest => dest.FavoriteProperties, opt => opt.Ignore())
                .ForMember(dest => dest.Messages, opt => opt.Ignore())
                .ForMember(dest => dest.Offers, opt => opt.Ignore());




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

            // Mapping para la API según especificación del documento
            CreateMap<Property, PropertyApiDto>()
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType.Name))
                .ForMember(dest => dest.SaleType, opt => opt.MapFrom(src => src.SaleType.Name))
                .ForMember(dest => dest.Improvements, opt => opt.MapFrom(src =>
                    src.PropertyImprovements.Select(pi => pi.Improvement.Name).ToList()))
                .ForMember(dest => dest.AgentName, opt => opt.Ignore())
                .ForMember(dest => dest.AgentId, opt => opt.MapFrom(src => src.AgentId));

        }
    }
}
