using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RealStateApp.Core.Application.Dtos.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface IPropertyService : IGenericService<Property, PropertyDto>
    {
        Task<int> GetAvailablePropertiesCount();
        Task<int> GetSoldPropertiesCount();
    }
}
