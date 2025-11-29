using RealStateApp.Core.Application.Dtos.User;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface IBaseAccountService
    {
        Task<UserResponseDto> DeleteAsync(string id);
        Task<EditUserResponseDto> EditUser(SaveUserDto saveDto, bool? isCreated = false);
        Task<RegisterUserResponseDto> RegisterAsync(SaveUserDto dto, string? origin);
    }
}