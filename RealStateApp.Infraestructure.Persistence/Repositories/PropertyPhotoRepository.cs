

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;

namespace RealStateApp.Infraestructure.Persistence.Repositories
{
    public class PropertyPhotoRepository : GenericRepository<PropertyPhoto>, IPropertyPhotoRepository
    {

        private readonly RealStateContext context;


        public PropertyPhotoRepository(RealStateContext context) : base(context)
        {
            this.context = context;

        }



        public async Task<List<PropertyPhoto>> GetListPhotByPropertyId(int PropertyId)
        {
            
            return await context.Set<PropertyPhoto>().Where(s => s.PropertyId == PropertyId).ToListAsync();

        }




        public async Task<PropertyPhoto?> GetPhotoByPropertyId(int PropertyId)
        {

            return await context.Set<PropertyPhoto>().FirstOrDefaultAsync(p => p.PropertyId == PropertyId);

        }

        public async Task<Property?> GetPropertyWithDetails(int propertyId)
        {
            return await context.Set<Property>()
                .Include(p => p.PropertyType)
                .Include(p => p.SaleType)
                .Include(p => p.Photos)
                .Include(p => p.PropertyImprovements)
                .FirstOrDefaultAsync(p => p.Id == propertyId);
        }
    }


}
