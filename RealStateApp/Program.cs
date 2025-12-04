using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.LayerConfigurations;
using RealStateApp.Infraestructure.Identity.LayerConfigurations;
using RealStateApp.Infraestructure.Persistence.LayerConfigurations;
using RealStateApp.Infraestructure.Persistence.Extensions;
using RealStateApp.Infraestructure.Shared.LayerConfigurations;
using System.Text.Json.Serialization;
using RealStateApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new ProducesAttribute("application/json"));
}).ConfigureApiBehaviorOptions(opt =>
{
    opt.SuppressInferBindingSourcesForParameters = true;
    opt.SuppressMapClientErrors = true;
})
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddApplicationLayerIOC();
builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.AddSharedLayer(builder.Configuration);
builder.Services.AddIdentityLayerForWebApp(builder.Configuration);

var app = builder.Build();

// Execute Seeds
await app.Services.RunIdentitySeedAsync();
await app.Services.RunPersistenceSeedAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<ChatHub>("/chatHub");

app.Run();
