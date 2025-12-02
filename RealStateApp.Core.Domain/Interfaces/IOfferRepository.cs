using RealStateApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Domain.Interfaces
{
    public interface IOfferRepository : IGenericRepository<Offer>
    {


        Task<List<Offer>> GetListByIdClientAndPropertyIdAsync(string ClientId, int propertyId);
        Task<List<Offer>> GetListByPropertyIdAsync(int propertyId);


    }
}
