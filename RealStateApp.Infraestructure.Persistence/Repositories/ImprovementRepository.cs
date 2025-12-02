using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Infraestructure.Persistence.Repositories
{
    public class ImprovementRepository : GenericRepository<Improvement>, IImprovementRepository


    {
        private readonly RealStateContext context;


        public ImprovementRepository(RealStateContext context) : base(context)
        {

            this.context = context;

        }

        public async Task<List<Improvement>> GetAllListById(int id)
        {
            return await context.Set<Improvement>().Where(s => s.Id == id).ToListAsync();
        }


     
    }
}
