using Microsoft.Extensions.DependencyInjection;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Repositories;


namespace RealStateApp.Infraestructure.Persistence.LayerConfigurations
{
    public static class ServicesRegistration
    {
        public static void AddPersistenceLayer(this IServiceCollection services)
        {


            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        }
    }
}
