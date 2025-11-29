using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Infraestructure.Persistence.Repositories
{
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        
        private RealStateContext _context;

        public PropertyRepository(RealStateContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, int>> GetAgentsPropertiesCount()
        {
            return await _context.Set<Property>()
                .GroupBy(p => p.AgentId)
                .Select(g => new
                {
                    AgentId = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.AgentId, x => x.Count);
        }

        public async Task DeleteAgentProperties(string AgentId)
        {
           var properties = await _context.Set<Property>().Where(r=>r.AgentId == AgentId).ToListAsync();

             _context.Set<Property>().RemoveRange(properties);

            _context.SaveChanges();
        }


    }
}
