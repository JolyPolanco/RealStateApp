using RealStateApp.Core.Application.Dtos.Login;
using RealStateApp.Core.Application.Dtos.User;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface IAccountServiceForWebApp : IBaseAccountService
    {
        Task<LoginResponseDto> AuthenticateAsync(LoginDto dto);
        Task<UserResponseDto> ConfirmAccountAsync(string token, string? userId = null);
        Task<UserDto?> GetById(string id);
        Task<UserDto?> GetByUserName(string name);
        Task<UserResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto dto);
        Task<UserResponseDto> ResetPasswordAsync(ResetPasswordRequestDto dto);
        Task<List<UserDto>> GetByNameAsync(string name);
        Task SignOutAsync();
    }
}
