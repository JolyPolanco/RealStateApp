

using AutoMapper;
using RealStateApp.Core.Application.Dtos.FavoriteProperty;
using RealStateApp.Core.Domain.Entities;

namespace RealStateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class FavoritePropertyToDtoMappingProfile : Profile
    {


        public FavoritePropertyToDtoMappingProfile()
        
        {


            CreateMap<FavoriteProperty, CreateFavoritePropertyDto>()
                .ReverseMap()
                .ForMember(s => s.Property, opt => opt.Ignore());
        
  
        }




    }
}
