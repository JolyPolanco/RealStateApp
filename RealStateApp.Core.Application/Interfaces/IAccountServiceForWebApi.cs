using RealStateApp.Core.Application.Dtos.Login;

namespace RealStateApp.Core.Application.Interfaces
{
    public interface IAccountServiceForWebApi : IBaseAccountService
    {
        Task<LoginResponseForApi> AuthenticateAsync(LoginDto loginDto);
    }
}
