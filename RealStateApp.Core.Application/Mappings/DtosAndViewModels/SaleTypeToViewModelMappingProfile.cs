using AutoMapper;
using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Application.ViewModels.SaleType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class SaleTypeToViewModelMappingProfile: Profile
    {
        public SaleTypeToViewModelMappingProfile()
        {
            CreateMap<SaleTypeDto,SaleTypeViewModel>();
        }
    }
}
