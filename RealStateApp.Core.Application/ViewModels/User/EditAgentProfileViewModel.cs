using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Core.Application.ViewModels.User
{
    public class EditAgentProfileViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Foto de perfil")]
        public IFormFile? PhotoFile { get; set; }

        public string? Photo { get; set; }

        public bool HasError { get; set; }
        public string? Error { get; set; }
    }
}
