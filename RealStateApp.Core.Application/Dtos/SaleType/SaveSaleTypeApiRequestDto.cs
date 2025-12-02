using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Dtos.SaleType
{
    public class SaveSaleTypeApiRequestDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
