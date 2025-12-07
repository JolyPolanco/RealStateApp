using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;


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

        public async Task<Property> GetByCode(string code)
        {
            return await _context.Set<Property>().FirstOrDefaultAsync(s => s.Code == code);
        }


        public async Task<List<Property>> GetListByAgentId(string AgenteId)
        {
            return await _context.Set<Property>().Where(s => s.AgentId == AgenteId
            && s.Status == Core.Domain.Common.Enums.PropertyStatus.Available).ToListAsync();
        }
        public async  Task<int> GetAgentPropertiesCount(string id)
        {
            return await _context.Set<Property>().Where(r=>r.AgentId == id).CountAsync();

           
        }

        public async Task<int> GetAvailablePropertiesCount()
        {
            return await _context.Set<Property>().Where(r => r.Status == PropertyStatus.Available).CountAsync();
        }

        public async Task<int> GetSoldPropertiesCount()
        {
            return await _context.Set<Property>().Where(r => r.Status == PropertyStatus.Sold).CountAsync();

        }
    }
}
