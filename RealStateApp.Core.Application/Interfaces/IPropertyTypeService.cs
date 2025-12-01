using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface IPropertyTypeService : IGenericService<PropertyType, PropertyTypeDto>
    {
        Task<List<PropertyTypeDto>> GetAllWithCount();
    }
}
