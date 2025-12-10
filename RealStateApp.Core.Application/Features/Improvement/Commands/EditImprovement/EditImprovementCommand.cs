using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Improvement.Commands.EditImprovement
{
    public class EditImprovementCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
    }


    public class EditImprovementCommandHandler : IRequestHandler<EditImprovementCommand, Unit>
    {
        private readonly IImprovementRepository _improvementRepository;

        public EditImprovementCommandHandler (IImprovementRepository improvementRepository)
        {
            _improvementRepository = improvementRepository;
        }
        public async Task<Unit> Handle(EditImprovementCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Improvement? getEntity = await _improvementRepository.GetByIdAsync(request.Id);

            if (getEntity == null)
                throw new ApiException("Entity not found with Id");

            Domain.Entities.Improvement? entity = new()
            {
                Description = request.Description,
                Name = request.Name,
                Id = request.Id
            };
            await _improvementRepository.UpdateAsync(request.Id, entity);
            return Unit.Value;

        }
    }

}
