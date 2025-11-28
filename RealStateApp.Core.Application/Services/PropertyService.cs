using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;

namespace RealStateApp.Core.Application.Services
{
    public class PropertyService :GenericService<Property,PropertyDto>,IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository, IMapper mapper) : base(propertyRepository, mapper)
        {
            _propertyRepository = propertyRepository;


        }

        public async Task <int> GetAvailablePropertiesCount()
        {
           return await _propertyRepository.GetAllQuery().Where(r=>r.Status==PropertyStatus.Available).CountAsync();
        }

        public async Task<int> GetSoldPropertiesCount()
        {
            return await _propertyRepository.GetAllQuery().Where(r => r.Status == PropertyStatus.Sold).CountAsync();
        }


    }
}
