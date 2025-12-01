
namespace RealStateApp.Core.Application.ViewModels.User
{
    public class AgentViewModel
    {
        public required string Id { get; set; }


        public required string Email { get; set; }
        public bool IsVerified { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }


        public bool IsActive { get; set; }

        public int PropertiesCount { get; set; }
    }
}
