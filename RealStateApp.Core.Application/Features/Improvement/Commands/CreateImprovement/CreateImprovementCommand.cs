using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Improvement.Commands.CreateImprovement
{

    /// <summary>
    /// Parameters for creating a property improvement
    /// </summary>
    public class CreateImprovementCommand:IRequest<int>
    {
        /// <example>Cámaras de seguridad</example>
        [SwaggerParameter(Description="Nombre de  la mejora")]
        public required string Name { get; set; }
        [SwaggerParameter(Description = "Descripcion de  la mejora")]

        public required string Description { get; set; }
    }

    public class CreateImprovementCommandHandler : IRequestHandler<CreateImprovementCommand, int>
    {

        private readonly IImprovementRepository _improvementRepository;
        public CreateImprovementCommandHandler(IImprovementRepository improvementRepository)
        {
            _improvementRepository = improvementRepository;
        }
        public async Task<int> Handle(CreateImprovementCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Improvement ?entity = new()
            {
                Id = 0,
                Description = request.Description,
                Name = request.Name,
            };
            entity = await _improvementRepository.AddAsync(entity);

            if (entity == null)
                throw new ApiException("Error creating entity", (int)HttpStatusCode.InternalServerError);
            return entity.Id;
        }
    }
}
