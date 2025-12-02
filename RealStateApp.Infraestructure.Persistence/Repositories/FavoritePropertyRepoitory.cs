

using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;

namespace RealStateApp.Infraestructure.Persistence.Repositories
{
    public class FavoritePropertyRepoitory : GenericRepository<FavoriteProperty>, IFavoritePropertyRepository
    {


        private readonly RealStateContext context;


        public FavoritePropertyRepoitory(RealStateContext context) : base(context)
        {

            this.context = context;

        }

        public async Task<FavoriteProperty?> GetFavoriteByIdClientAndByIdProperty(string clientId, int propertyid)
        {



            return await context.Set<FavoriteProperty>()
                .FirstOrDefaultAsync(s => s.ClientId == clientId 
                && s.PropertyId == propertyid);



        }

      
        
        public async Task<List<FavoriteProperty>?> GetListFavoriteByIdClient(string clientId)
        {
            return await context.Set<FavoriteProperty>().Where(s => s.ClientId == clientId).ToListAsync();
        }
    }
}
