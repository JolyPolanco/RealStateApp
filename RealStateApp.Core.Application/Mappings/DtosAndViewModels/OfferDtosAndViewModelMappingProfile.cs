using AutoMapper;
using RealStateApp.Core.Application.Dtos.Offer;
using RealStateApp.Core.Application.ViewModels.offer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class OfferDtosAndViewModelMappingProfile : Profile
    {


        public OfferDtosAndViewModelMappingProfile()
        {


            CreateMap<DataListOfferDto, DataOfferViewModel>()
                .ReverseMap();


            CreateMap<CreateOfferDto, CreateOfferViewModel>()
                .ReverseMap();
           
        
        
        
        }







    }
}
