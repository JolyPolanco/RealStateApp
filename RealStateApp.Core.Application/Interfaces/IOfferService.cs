

using RealStateApp.Core.Application.Dtos.Offer;
using RealStateApp.Core.Domain.Entities;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface IOfferService : IGenericService<Offer,CreateOfferDto>
    {


        Task<List<DataListOfferDto>> GetListByIdClientAndPropertyIdAsync(string ClientId,int propertyId);
        Task<bool> IsOfferActive(string clientId, int PropertyId);

    }
}
