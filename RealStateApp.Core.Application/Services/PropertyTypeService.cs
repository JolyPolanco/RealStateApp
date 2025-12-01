using AutoMapper;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Services
{
    public class PropertyTypeService : GenericService<PropertyType, PropertyTypeDto>, IPropertyTypeService
    {
        private readonly IMapper _mapper;
        private IPropertyTypeRepository _propertyTypeRepository;
        public PropertyTypeService(IPropertyTypeRepository repo, IMapper mapper) : base(repo, mapper)
        {
            _mapper = mapper;
            _propertyTypeRepository = repo;
            
        }

        public async Task<List<PropertyTypeDto>> GetAllWithCount()
        {
            var list = await _propertyTypeRepository
                .GetAllListWithInclude(new List<string> { "Properties" });

            return _mapper.Map<List<PropertyTypeDto>>(list);
        }

    }
}
