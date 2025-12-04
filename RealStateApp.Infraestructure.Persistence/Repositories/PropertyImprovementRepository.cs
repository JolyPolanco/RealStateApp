

using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;

namespace RealStateApp.Infraestructure.Persistence.Repositories
{
    public class PropertyImprovementRepository : GenericRepository<PropertyImprovement>, IPropertyImprovementRepository
    {

        private readonly RealStateContext context;

        public PropertyImprovementRepository(RealStateContext context) : base(context)
        {


            this.context = context;
        }



        public async Task<PropertyImprovement> GetPropertyImprovementByPropertyId(int PropertyId)
        {

            return await context!.Set<PropertyImprovement>().FirstOrDefaultAsync(s => s.PropertyId == PropertyId);

        }


        public async Task<List<PropertyImprovement>> GetListImprovementBYPorpertyId(int PropertyId)
        {

            return await context!.Set<PropertyImprovement>()
                .Where(s => s.PropertyId == PropertyId)
                .ToListAsync();

        }

        public async Task DeleteByPropertyId(int propertyId)
        {
            var improvements = await context.Set<PropertyImprovement>()
                .Where(pi => pi.PropertyId == propertyId)
                .ToListAsync();
            
            context.Set<PropertyImprovement>().RemoveRange(improvements);
            await context.SaveChangesAsync();
        }
    }
}
