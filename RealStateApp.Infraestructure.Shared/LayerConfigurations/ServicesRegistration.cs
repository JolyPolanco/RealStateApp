using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Core.Domain.Settings;
using RealStateApp.Infraestructure.Shared.Services;
using static Org.BouncyCastle.Math.EC.ECCurve;


namespace RealStateApp.Infraestructure.Shared.LayerConfigurations
{
    public static class ServicesRegistration
    {
        public static void AddSharedLayer(this IServiceCollection services, IConfiguration config)
        {

            services.Configure<MailSettings>(config.GetSection("MailSettings"));

            services.AddScoped<IEmailService, EmailService> ();

        }
    }
}
