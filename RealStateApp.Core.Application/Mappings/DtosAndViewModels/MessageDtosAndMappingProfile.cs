

using AutoMapper;
using RealStateApp.Core.Application.Dtos.Message;
using RealStateApp.Core.Application.ViewModels.Message;
using RealStateApp.Core.Domain.Entities;
using System.Runtime.InteropServices;

namespace RealStateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class MessageAndDtoMappingProfile : Profile
    {

        public MessageAndDtoMappingProfile()
        {
        
        
           
             CreateMap<CreateMessageDto, MessageAndDtoMappingProfile>()
                .ReverseMap();
           
             CreateMap<CreateMessageDto, CreateMessageViewModel>()
                .ReverseMap();
           
             CreateMap<DataConversactionDto, DataConversactionViewModel>()
                .ReverseMap();


            CreateMap<Message, CreateMessageDto>()
               .ReverseMap();






        }




    }
}
