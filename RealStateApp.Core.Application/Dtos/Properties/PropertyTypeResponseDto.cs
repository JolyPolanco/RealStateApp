using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Dtos.Properties
{
    public class PropertyTypeResponseDto
    {
        public List<PropertyTypeDto> PropertyTypes { get; set; } = new List<PropertyTypeDto>();
    }
}
