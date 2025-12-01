using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Queries.GetById
{
    public class GetPropertyTypeByIdQuery : IRequest<PropertyTypeDto>
    {
        public required int Id { get; set; }
    }
    public class GetSaleTypeByIdQueryHandler : IRequestHandler<GetPropertyTypeByIdQuery, PropertyTypeDto>
    {
        private readonly ISaleTypeRepository _repository;
        private readonly IMapper _mapper;
        public GetSaleTypeByIdQueryHandler(ISaleTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PropertyTypeDto> Handle(GetPropertyTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _repository.GetAllQueryWithInclude(new List<string> { "Properties" });
            var entity = await listEntitiesQuery.FirstOrDefaultAsync(fd => fd.Id == request.Id, cancellationToken: cancellationToken);
            if (entity == null) throw new ArgumentException("El id es inválido");

            var dto = _mapper.Map<PropertyTypeDto>(entity);
            return dto;
        }
    }
}
