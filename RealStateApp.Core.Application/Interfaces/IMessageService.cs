

using RealStateApp.Core.Application.Dtos.Message;
using RealStateApp.Core.Domain.Entities;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface IMessageService : IGenericService<Message,CreateMessageDto>
    {

        Task<List<DataConversactionDto>> GetConversaction(int propertyId,string clientId, string agentId); 


    }
}
