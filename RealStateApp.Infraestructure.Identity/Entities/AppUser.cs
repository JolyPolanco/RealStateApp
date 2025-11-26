using Microsoft.AspNetCore.Identity;

namespace RealStateApp.Infraestructure.Identity.Entities
{
    public class AppUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Photo { get; set; }
        public string? Dni { get; set; }
    }
}


