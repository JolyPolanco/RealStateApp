
using RealStateApp.Core.Application.LayerConfigurations;
using RealStateApp.Infraestructure.Identity.LayerConfigurations;
using RealStateApp.Infraestructure.Persistence.LayerConfigurations;
using RealStateApp.Infraestructure.Shared.LayerConfigurations;
using RealStateWebApi.Extensions;
using RealStateWebApi.Handlers;
using System.Globalization;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var cultureInfo = new CultureInfo("en-US");
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
builder.Services.AddControllers()
                .AddNewtonsoftJson();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllersWithViews();
builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.AddSharedLayer(builder.Configuration);
builder.Services.AddApplicationLayerIOC();
builder.Services.AddIdentityLayerForWebApi(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();



builder.Services.AddSwaggerExtension();
builder.Services.AddApiVersioningExtension();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler> ();

var app = builder.Build();
await app.Services.RunIdentitySeedAsync();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerExtension(app);
}

app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseHealthChecks("/health");
app.MapControllers();

await app.RunAsync();
