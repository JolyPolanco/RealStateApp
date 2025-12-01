using MediatR;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Improvement.Commands.CreateImprovement
{

    /// <summary>
    /// Parameters for creating a property improvement
    /// </summary>
    public class CreateImpromentCommand:IRequest<int>
    {
        /// <example>Cámaras de seguridad</example>
        [SwaggerParameter(Description="Nombre de  la mejora")]
        public required string Name { get; set; }
        [SwaggerParameter(Description = "Descripcion de  la mejora")]

        public required string Description { get; set; }
    }

    public class CreateImprovementCommandHandler : IRequestHandler<CreateImpromentCommand, int>
    {

        private readonly IImprovementRepository _improvementRepository;
        public CreateImprovementCommandHandler(IImprovementRepository improvementRepository)
        {
            _improvementRepository = improvementRepository;
        }
        public async Task<int> Handle(CreateImpromentCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Improvement entity = new()
            {
                Id = 0,
                Description = request.Description,
                Name = request.Name,
            };
            entity = await _improvementRepository.AddAsync(entity);
           return entity != null  ? entity.Id : 0;
        }
    }
}
