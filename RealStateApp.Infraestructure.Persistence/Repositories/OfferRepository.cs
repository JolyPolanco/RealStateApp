

using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;

namespace RealStateApp.Infraestructure.Persistence.Repositories
{
    public class OfferRepository : GenericRepository<Offer>, IOfferRepository
    {
        private readonly RealStateContext context;


        public OfferRepository(RealStateContext context) : base(context)
        {

            this.context = context;


        }



        public async Task<List<Offer>> GetListByIdClientAndPropertyIdAsync(string ClientId, int propertyId)
        {


            return await context.Set<Offer>()
                .Where(s => s.ClientId == ClientId && s.PropertyId == propertyId)
                .ToListAsync();



        }




        public async Task<List<Offer>> GetListByPropertyIdAsync(int propertyId)
        {

            return await context.Set<Offer>()
                .Where(s => s.PropertyId == propertyId)
                .ToListAsync();
        }
    }
}
