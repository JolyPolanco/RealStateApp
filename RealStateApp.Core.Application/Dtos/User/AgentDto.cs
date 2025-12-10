using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Dtos.User
{
    public class AgentDto
    {
        public required string Id { get; set; }

        [JsonProperty("correo")]

        public required string Email { get; set; }
        [JsonIgnore]
        public bool IsVerified { get; set; }
        [JsonProperty("nombre")]

        public required string FirstName { get; set; }
        [JsonProperty("apellido")]
        public required string LastName { get; set; }

        [JsonProperty("telefono")]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public int PropertiesCount { get; set; }
    }
}
