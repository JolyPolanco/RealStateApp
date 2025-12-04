using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealStateApp.Core.Application.Dtos.Login;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Settings;
using RealStateApp.Infraestructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Infraestructure.Identity.Services
{
    public class AccountServiceForWebApi : BaseAccountService, IAccountServiceForWebApi
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public AccountServiceForWebApi(UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, IOptions<JwtSettings> jwtSettings, IMapper mapper) : base(userManager, emailService, signInManager, mapper)
        {
            _userManager = userManager;
            _signInManager= signInManager;
            _jwtSettings = jwtSettings.Value;

        }

        public async Task<LoginResponseForApi> AuthenticateAsync(LoginDto loginDto)
        {

            LoginResponseForApi responseDto = new LoginResponseForApi() { LastName = "", Name = "", HasError = false };
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                responseDto.HasError = true;
                responseDto.Errors!.Add($"No hay ningun usuario el nombre de usuario {loginDto.Username}");
                return responseDto;
            }
            if (!user.EmailConfirmed)
            {
                responseDto.HasError = true;
                responseDto.Errors!.Add($"Esta cuenta no esta activa. Actívala mediante un link que ha sido enviado a tu correo");
                return responseDto;
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName ?? "", loginDto.Password, false, false);
            if (!result.Succeeded)
            {
                responseDto.HasError = true;
                responseDto.Errors!.Add($"Usuario o contraseña incorrectos");
                return responseDto;

            }
            JwtSecurityToken jwtSecurityToken =  GenerateJwtToken(user);

            var rolesList = await _userManager.GetRolesAsync(user);


            responseDto.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return responseDto;
        }
        private JwtSecurityToken GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("uid", user.Id)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: creds
            );
        }

    }
}
