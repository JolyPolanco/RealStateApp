

using AutoMapper;
using RealStateApp.Core.Application.Dtos.FavoriteProperty;
using RealStateApp.Core.Application.ViewModels.FavoriteProperty;

namespace RealStateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class FavoritePropertyDtosAndMappingProfile : Profile
    {



        public FavoritePropertyDtosAndMappingProfile()
        {


            CreateMap<CreateFavoritePropertyDto, CreateFavoriteViewModel>()
               .ReverseMap();
        
        
        
        
        
        }





    }
}
