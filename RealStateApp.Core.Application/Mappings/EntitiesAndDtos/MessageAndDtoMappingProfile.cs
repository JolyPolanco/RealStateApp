using AutoMapper;
using RealStateApp.Core.Application.Dtos.Message;
using RealStateApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class MessageAndDtoMappingProfile : Profile
    {


        public MessageAndDtoMappingProfile()
        {

            CreateMap<Message, CreateMessageDto>()
                 .ReverseMap();
        
        
        
        }





    }
}
