

using AutoMapper;
using RealStateApp.Core.Application.Dtos.Message;
using RealStateApp.Core.Application.ViewModels.Message;
using System.Runtime.InteropServices;

namespace RealStateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class MessageDtosAndMappingProfile : Profile
    {

        public MessageDtosAndMappingProfile()
        {
        
        
           
             CreateMap<CreateMessageDto, MessageDtosAndMappingProfile>()
                .ReverseMap();
           
             CreateMap<CreateMessageDto, CreateMessageViewModel>()
                .ReverseMap();
           
             CreateMap<DataConversactionDto, DataConversactionViewModel>()
                .ReverseMap();
        
        
        
        
        
        }




    }
}
