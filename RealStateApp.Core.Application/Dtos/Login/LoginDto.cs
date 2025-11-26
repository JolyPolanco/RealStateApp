using Newtonsoft.Json;

namespace RealStateApp.Core.Application.Dtos.Login
{
    public class LoginDto
    {
        [JsonProperty("usuario")]
        public required string Username { get; set; }
        [JsonProperty("contrasena")]

        public required string Password { get; set; }
    }
}
