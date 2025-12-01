

namespace RealStateApp.Core.Application.Dtos.Message
{
    public class DataConversactionDto
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public DateTime Date { get; set; }

        public int PropertyId { get; set; }
        public required string SenderUserId { get; set; }
        public required string ReceiverUserId { get; set; }
    }
}
