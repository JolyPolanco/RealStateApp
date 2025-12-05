using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Application.Features.Improvement.Queries.GetAll;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Improvement.Queries.GetAllWithInclude
{
    public class GetAllImprovementWithIncludeQuery: IRequest<IList<ImprovementDto>>
    {
    
    }

    public class GetAllWithIncludeImprovementQueryHandler : IRequestHandler<GetAllImprovementWithIncludeQuery, IList<ImprovementDto>>
    {
        private readonly IImprovementRepository _improvementRepository;
        private readonly IMapper _mapper;

        public GetAllWithIncludeImprovementQueryHandler(IImprovementRepository improvementRepository, IMapper mapper)
        {
            _improvementRepository = improvementRepository;
            _mapper = mapper;
        }
        public async Task<IList<ImprovementDto>> Handle(GetAllImprovementWithIncludeQuery request, CancellationToken cancellationToken)
        {
            var listEntitiesQuery = _improvementRepository.GetAllQueryWithInclude(new List<string> { "PropertyImprovements" });

            var listEntityDtos = await listEntitiesQuery.ProjectTo<ImprovementDto>(_mapper.ConfigurationProvider).ToListAsync();

            return listEntityDtos;
        }
    }
}
