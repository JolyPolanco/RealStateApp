using RealStateApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Dtos.FavoriteProperty
{
    public class CreateFavoritePropertyDto
    {


        public int Id { get; set; }
        public required string ClientId { get; set; }
        public int PropertyId { get; set; }
      


    }
}
