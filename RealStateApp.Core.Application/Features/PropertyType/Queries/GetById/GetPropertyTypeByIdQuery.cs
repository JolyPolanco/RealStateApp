using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using System.Net;


namespace RealStateApp.Core.Application.Features.PropertyType.Queries.GetById
{
    public class GetPropertyTypeByIdQuery : IRequest<PropertyTypeDto>
    {
        public required int Id { get; set; }
    }
    public class GetPropertyTypeByIdQueryHandler : IRequestHandler<GetPropertyTypeByIdQuery, PropertyTypeDto>
    {
        private readonly IPropertyTypeRepository _repository;
        private readonly IMapper _mapper;
        public GetPropertyTypeByIdQueryHandler(IPropertyTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PropertyTypeDto> Handle(GetPropertyTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _repository.GetAllQueryWithInclude(new List<string> { "Properties" });
            var entity = await listEntitiesQuery.FirstOrDefaultAsync(fd => fd.Id == request.Id, cancellationToken: cancellationToken);
            if (entity == null) throw new ApiException("Invalid Id",(int)HttpStatusCode.NotFound);

            var dto = _mapper.Map<PropertyTypeDto>(entity);
            return dto;
        }
    }
}
