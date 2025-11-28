using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealStateApp.Core.Domain.Entities;
namespace RealStateApp.Core.Application.Dtos.Properties
{
    public class PropertyDto
    {
        
            public int Id { get; set; }
            public required string Code { get; set; }
            public decimal Price { get; set; }
            public double SizeInMeters { get; set; }
            public int Bedrooms { get; set; }
            public int Bathrooms { get; set; }
            public required string Description { get; set; }
            public PropertyStatus Status { get; set; } = PropertyStatus.Available;

            public int PropertyTypeId { get; set; }
            public int SaleTypeId { get; set; }
            public required string AgentId { get; set; }

            public PropertyType PropertyType { get; set; } = null!;
            public SaleType SaleType { get; set; } = null!;

          
        }
    }



