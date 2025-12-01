using AutoMapper;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Application.ViewModels.Improvement;
using RealStateApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Mappings.ViewModelsAndDtos
{
    public class ImprovementToViewModelMappingProfile: Profile
    {
        public ImprovementToViewModelMappingProfile()
        {
            CreateMap<ImprovementDto, ImprovementViewModel>()
                .ReverseMap();
        }
    }
}
