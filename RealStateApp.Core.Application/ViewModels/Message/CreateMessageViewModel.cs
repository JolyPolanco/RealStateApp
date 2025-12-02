

using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Core.Application.ViewModels.Message
{
    public class CreateMessageViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debes introducir un texto para poder enviar")]
        public required string Content { get; set; }
        public DateTime Date { get; set; }

        public int PropertyId { get; set; }
        public required string SenderUserId { get; set; }
        public required string ReceiverUserId { get; set; }

    }
}
