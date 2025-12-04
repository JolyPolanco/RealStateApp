using RealStateApp.Core.Domain.Entities;


namespace RealStateApp.Core.Domain.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<List<Message>> GetMessagesByPropertyId(int propertyId);
        Task<List<Message>> GetMessagesByPropertyAndClient(int propertyId, string clientId);
    }
}
