using RealStateApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Domain.Interfaces
{
    public interface IFavoritePropertyRepository : IGenericRepository<FavoriteProperty>
    {

        Task<FavoriteProperty?> GetFavoriteByIdClientAndByIdProperty(string clientId, int propertyid);
        Task<List<FavoriteProperty>?> GetListFavoriteByIdClient(string clientId);

    }
}
