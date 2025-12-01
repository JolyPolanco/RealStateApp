

using AutoMapper;
using RealStateApp.Core.Application.Dtos.Offer;
using RealStateApp.Core.Domain.Entities;

namespace RealStateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class OfferToDtoMappingProfile : Profile
    {

        public OfferToDtoMappingProfile() 
        {


            CreateMap<Offer, CreateOfferDto>()
               .ForMember(s => s.DisableButton, opt => opt.Ignore())
               .ReverseMap()
               .ForMember(s => s.Property, opt => opt.Ignore());
           
            CreateMap<Offer, DataListOfferDto>()
               .ReverseMap()
               .ForMember(s => s.Property, opt => opt.Ignore());
           
              
        
        
        
        }   

    }
}
