

using RealStateApp.Core.Application.Dtos.FavoriteProperty;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Domain.Entities;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface IFavoritePropertyService : IGenericService<FavoriteProperty,CreateFavoritePropertyDto>
    {



        Task<List<DataPropertyDto>> GetListPropertyFavoriteAsync(string client);
       

    }
}
