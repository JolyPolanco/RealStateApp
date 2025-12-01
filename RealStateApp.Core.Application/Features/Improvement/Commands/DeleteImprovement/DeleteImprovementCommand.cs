using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RealStateApp.Core.Application.Features.Improvement.Commands.CreateImprovement;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Improvement.Commands.DeleteImprovement
{
    public class DeleteImprovementCommand : IRequest<Unit>
    {

        public required  int Id { get; set; }
        
    }

    public class DeleteImprovementCommandHandler : IRequestHandler<DeleteImprovementCommand, Unit>
    {
        private readonly IImprovementRepository _improvementRepository;

        public DeleteImprovementCommandHandler(IImprovementRepository improvementRepository)
        {
            _improvementRepository = improvementRepository;
        }

        public async Task<Unit> Handle(DeleteImprovementCommand request, CancellationToken cancellationToken)
        {
            var entity = await _improvementRepository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new ArgumentNullException("Mejora no encontrada con ese Id");

            await _improvementRepository.DeleteAsync(request.Id);
            return Unit.Value;
        }
    }


}
