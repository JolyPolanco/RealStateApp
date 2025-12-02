using RealStateApp.Core.Domain.Entities;


namespace RealStateApp.Core.Domain.Interfaces
{
    public interface IPropertyPhotoRepository : IGenericRepository<PropertyPhoto>
    {



        Task<List<PropertyPhoto>> GetListPhotByPropertyId(int PropertyId);

        Task<PropertyPhoto?> GetPhotoByPropertyId(int PropertyId);


    }
}
