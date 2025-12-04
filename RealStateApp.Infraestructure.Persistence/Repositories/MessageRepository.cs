using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Infraestructure.Persistence.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly RealStateContext _context;

        public MessageRepository(RealStateContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Message>> GetMessagesByPropertyId(int propertyId)
        {
            return await _context.Set<Message>()
                .Where(m => m.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task<List<Message>> GetMessagesByPropertyAndClient(int propertyId, string clientId)
        {
            return await _context.Set<Message>()
                .Where(m => m.PropertyId == propertyId && 
                           (m.SenderUserId == clientId || m.ReceiverUserId == clientId))
                .OrderBy(m => m.Date)
                .ToListAsync();
        }
    }
}
