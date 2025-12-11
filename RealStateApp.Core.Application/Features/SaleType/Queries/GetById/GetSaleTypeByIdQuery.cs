using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.SaleType.Queries.GetById
{
    public class GetSaleTypeByIdQuery : IRequest<SaleTypeDto>
    {
        public required int Id { get; set; }
    }
    public class GetSaleTypeByIdQueryHandler : IRequestHandler<GetSaleTypeByIdQuery, SaleTypeDto>
    {
        private readonly ISaleTypeRepository _repository;
        private readonly IMapper _mapper;
        public GetSaleTypeByIdQueryHandler(ISaleTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SaleTypeDto> Handle(GetSaleTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _repository.GetAllQueryWithInclude(new List<string> { "Properties" });
            var entity = await listEntitiesQuery.FirstOrDefaultAsync(fd => fd.Id == request.Id, cancellationToken: cancellationToken);
            if (entity == null) throw new ApiException("Sale Type not found with Id",(int)HttpStatusCode.NotFound);

            var dto = _mapper.Map<SaleTypeDto>(entity);
            return dto;
        }
    }
}
