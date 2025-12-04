

using RealStateApp.Core.Domain.Entities;

namespace RealStateApp.Core.Domain.Interfaces
{
    public interface IPropertyImprovementRepository : IGenericRepository<PropertyImprovement>
    {

        Task<PropertyImprovement> GetPropertyImprovementByPropertyId(int PropertyId);
        Task<List<PropertyImprovement>> GetListImprovementBYPorpertyId(int PropertyId);
        Task DeleteByPropertyId(int propertyId);
    }
}
