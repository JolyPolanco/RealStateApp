

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
    }


}
