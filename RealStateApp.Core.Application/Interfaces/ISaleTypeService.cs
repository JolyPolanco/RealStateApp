using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface ISaleTypeService : IGenericService<SaleType,SaleTypeDto>
    {
    }
}
