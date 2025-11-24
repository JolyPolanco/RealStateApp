using RealStateApp.Core.Application.LayerConfigurations;
using RealStateApp.Infraestructure.Persistence.LayerConfigurations;
using RealStateApp.Infraestructure.Shared.LayerConfigurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplicationLayerIOC();
builder.Services.AddPersistenceLayer();
builder.Services.AddSharedLayer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
