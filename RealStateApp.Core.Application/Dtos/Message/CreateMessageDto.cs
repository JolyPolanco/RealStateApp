using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Dtos.Message
{
    public class CreateMessageDto
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public DateTime Date { get; set; }

        public int PropertyId { get; set; }
        public required string SenderUserId { get; set; }
        public required string ReceiverUserId { get; set; }

    }
}
