using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RealStateApp.Core.Application.Dtos.Login;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Infraestructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Infraestructure.Identity.Services
{
    public class AccountServiceForWebApp : BaseAccountService, IAccountServiceForWebApp
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountServiceForWebApp(UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager) : base(userManager, emailService, signInManager)
        {
            _userManager = userManager;
            _signInManager=signInManager;
        }
        public virtual async Task<UserResponseDto> ConfirmAccountAsync(string token, string? userId = null)
        {
            UserResponseDto response = new()
            {
                HasError = false,
                Errors = new List<string>()
            };

            if (string.IsNullOrEmpty(userId))
            {
                response.Message = "No se proporcionó un UserId válido";
                response.HasError = true;
                return response;
            }

            try
            {
                token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            }
            catch
            {
                response.Message = "El token proporcionado es inválido o está corrupto";
                response.HasError = true;
                return response;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.Message = "No existe ninguna cuenta asociada a este usuario";
                response.HasError = true;
                return response;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                user.IsActive = true;
                await _userManager.UpdateAsync(user);

                response.Message = $"Cuenta confirmada correctamente para {user.Email}. Ya puedes iniciar sesión.";
                return response;
            }

            response.Message = $"Hubo un error al confirmar la cuenta de {user.Email}.";
            response.HasError = true;

            return response;
        }
        public async Task SignOutAsync()
        {

            await _signInManager.SignOutAsync();
        }

        public async Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto)
        {

            LoginResponseDto responseDto = new LoginResponseDto() { Email = "", Id = "", UserName = "", HasError = false };
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                responseDto.HasError = true;
                responseDto.Error = $"No hay ningún usuario con el nombre de usuario {loginDto.Username}";
                return responseDto;
            }

            if (!user.EmailConfirmed)
            {
                responseDto.HasError = true;
                responseDto.Error = $"Esta cuenta no está activa. Actívala mediante el link que ha sido enviado a tu correo";
                return responseDto;


            }

            if (!user.IsActive)
            {
                responseDto.HasError = true;
                responseDto.Error = $"Esta cuenta está desactivada. Contacta al administrador para más información";
                return responseDto;
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName ?? "", loginDto.Password, false, false);
            if (!result.Succeeded)
            {
                responseDto.HasError = true;
                responseDto.Error = $"Usuario o contraseña incorrectos";
                return responseDto;

            }
            var rolesList = await _userManager.GetRolesAsync(user);
            responseDto.Id = user.Id;
            responseDto.UserName = user.UserName ?? "";
            responseDto.Email = user.Email ?? "";
            responseDto.IsVerified = user.EmailConfirmed && user.IsActive;
            responseDto.Roles = rolesList.ToList();


            return responseDto;
        }
    }
}
