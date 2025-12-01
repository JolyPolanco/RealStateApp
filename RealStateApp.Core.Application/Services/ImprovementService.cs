using AutoMapper;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;


namespace RealStateApp.Core.Application.Services
{
    public class ImprovementService : GenericService<Improvement, ImprovementDto>, IImpromentService
    {
        public ImprovementService(IGenericRepository<Improvement> repo, IMapper mapper) : base(repo, mapper)
        {
        }
    }
}
