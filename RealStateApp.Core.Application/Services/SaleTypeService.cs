using AutoMapper;
using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Services
{
    public class SaleTypeService : GenericService<SaleType, SaleTypeDto>, ISaleTypeService
    {
        public SaleTypeService(IGenericRepository<SaleType> repo, IMapper mapper) : base(repo, mapper)
        {
        }
    }
}
