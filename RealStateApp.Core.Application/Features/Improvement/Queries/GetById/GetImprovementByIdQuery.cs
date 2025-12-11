using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Improvement.Queries.GetById
{
    public class GetImprovementByIdQuery: IRequest<ImprovementDto>
    {
        public required int Id { get; set; }
    }
    public class GetImprovementByIdQueryHandler : IRequestHandler<GetImprovementByIdQuery, ImprovementDto>
    {
        private readonly IImprovementRepository _improvementRepository;
        private readonly IMapper _mapper;
        public GetImprovementByIdQueryHandler(IImprovementRepository improvementRepository, IMapper mapper)
        {
            _improvementRepository = improvementRepository;
            _mapper = mapper;
        }

        public async Task<ImprovementDto> Handle(GetImprovementByIdQuery request, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _improvementRepository.GetAllQueryWithInclude(new List<string> { "PropertyImprovements" });
             var entity= await listEntitiesQuery.FirstOrDefaultAsync(fd=>fd.Id==request.Id, cancellationToken:cancellationToken);
            if (entity == null) throw new ApiException("Entity Not found with this id",(int)HttpStatusCode.NotFound);

            var dto= _mapper.Map<ImprovementDto>(entity);
            return dto;
        }
    }
}
