using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Dtos.User;
using System.Threading.Tasks;


namespace RealStateApp.Core.Application.Interfaces
{
    public interface IHomeClienteService : IGenericService<Property,PropertyDto>
    {

        Task<List<DataPropertyDto>> ListProperty(string ClietID);
        Task<DataPropertyDto?> FilterByCode(string? code = null, string? userId = null);
        Task<List<DataPropertyDto>> FilterMultiple(string? propertyType,decimal? priceMIN,decimal? priceMax, int? Bedrooms ,int? Bathrooms, string? userId);

        Task<DetailsPropertyDto?> ListDetailProperty(int propertyId);
        Task<List<AgentDataDto>> ListAgentAsync();
        Task<List<DataPropertyDto>> ListPropertyAgentAsync(string Agent, string clientId);
        Task<List<AgentDataDto>> GetAgentByName(string Name);

    }
}
