using System.ComponentModel.DataAnnotations;

namespace RealStateApp.Core.Application.ViewModels.User
{
    public class SaveBasicUserViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "El correo es requerido.")]
        [EmailAddress(ErrorMessage = "Debe ser un correo válido.")]
        public required string Email { get; set; }

        public bool IsVerified { get; set; }

        [Required(ErrorMessage = "El nombre es requerido.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido.")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [MinLength(8, ErrorMessage = "Debe tener al menos 8 caracteres.")]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':"",.<>/?]).+$",
            ErrorMessage = "Debe contener una mayúscula, un número y un carácter especial.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "El DNI es requerido.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Debe contener 11 dígitos numéricos.")]
        public required string Dni { get; set; }

        public required string Role { get; set; }

        public bool IsActive { get; set; }
    }
}
