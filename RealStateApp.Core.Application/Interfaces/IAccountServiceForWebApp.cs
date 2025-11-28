using RealStateApp.Core.Application.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface IAccountServiceForWebApp : IBaseAccountService
    {
        Task<UserResponseDto> ConfirmAccountAsync(string token, string? userId = null);
        Task<UserDto?> GetById(string id);
        Task<UserDto?> GetByUserName(string name);
        Task SignOutAsync();
    }
}
