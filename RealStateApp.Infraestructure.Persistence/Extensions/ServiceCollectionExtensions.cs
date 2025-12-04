using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Seeds;

namespace RealStateApp.Infraestructure.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static async Task RunPersistenceSeedAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RealStateContext>();
            
            // Ensure database is created and migrations are applied
            await context.Database.MigrateAsync();
            
            // Seed default data
            await DefaultPropertyData.SeedAsync(context);
        }
    }
}
