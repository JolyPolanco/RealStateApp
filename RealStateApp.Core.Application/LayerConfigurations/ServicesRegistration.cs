using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RealStateApp.Core.Application.Behaviors;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.Services;
using System.Reflection;



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

            services.AddScoped<IAdministrationService, AdministrationService>();
            services.AddScoped<IImpromentService, ImprovementService>();
            services.AddScoped<ISaleTypeService, SaleTypeService>();


            services.AddScoped<IFavoritePropertyService, FavoritePropertyService>();
            services.AddScoped<IOfferService, OfferService>();
            services.AddScoped<IHomeClienteService, HomeClientService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IAgentService, AgentService>();
      

            services.AddMediatR(opt => opt.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));





        }
    }
}
