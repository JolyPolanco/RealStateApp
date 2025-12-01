using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Dtos.SaleType
{
    public class SaleTypeResponseDto
    {
        public List<SaleTypeDto> Sales { get; set; }= new List<SaleTypeDto>();
    }
}
