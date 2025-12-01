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

        public MessageRepository(RealStateContext context) : base(context)
        {
        }
    }
}
