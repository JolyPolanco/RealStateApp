using Microsoft.Extensions.DependencyInjection;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Shared.Services;


namespace RealStateApp.Infraestructure.Shared.LayerConfigurations
{
    public static class ServicesRegistration
    {
        public static void AddSharedLayer(this IServiceCollection services)
        {


            services.AddScoped<IEmailService, EmailService> ();

        }
    }
}
