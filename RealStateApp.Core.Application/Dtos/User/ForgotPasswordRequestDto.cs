namespace RealStateApp.Core.Application.Dtos.User
{
    public class ForgotPasswordRequestDto
    {
        public required string Username { get; set; }
        public required string Origin { get; set; }
    }
}
