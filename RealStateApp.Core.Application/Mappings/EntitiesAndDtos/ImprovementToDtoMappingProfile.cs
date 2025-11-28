using AutoMapper;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class ImprovementToDtoMappingProfile: Profile
    {
        public ImprovementToDtoMappingProfile()
        {
            CreateMap<Improvement, ImprovementDto>();
        }
    }
}
