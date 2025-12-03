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
        public Task<int> GetAgentPropertiesCount(string id);
        Task<Dictionary<string, int>> GetAgentsPropertiesCount();
        Task<int> GetAvailablePropertiesCount();
        Task<Property> GetByCode(string code);
        Task<int> GetSoldPropertiesCount();
    }
}
