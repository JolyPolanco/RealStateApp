using Microsoft.Extensions.DependencyInjection;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.Services;

using System.Reflection;
using System.Text;

namespace RealStateApp.Core.Application.LayerConfigurations
{
    public static class ServicesRegistration
    {

        public static void AddApplicationLayerIOC(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));

            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IPropertyTypeService, PropertyTypeService>();

            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IAdministrationService, AdministrationService>();
            services.AddScoped<IImpromentService, ImprovementService>();
            services.AddScoped<ISaleTypeService, SaleTypeService>();
            services.AddMediatR(opt => opt.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        }
    }
}
