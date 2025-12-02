using RealStateApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Domain.Interfaces
{
    public interface IPropertyRepository : IGenericRepository<Property>
    {
        Task DeleteAgentProperties(string AgentId);
        Task<Dictionary<string, int>> GetAgentsPropertiesCount();

        Task<Property> GetByCode(string code);
      
    }
}
