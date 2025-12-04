using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;

namespace RealStateApp.Infraestructure.Persistence.Seeds
{
    public static class DefaultPropertyData
    {
        public static async Task SeedAsync(RealStateContext context)
        {
            // Seed Property Types
            if (!await context.PropertyTypes.AnyAsync())
            {
                var propertyTypes = new List<PropertyType>
                {
                    new PropertyType { Name = "Casa", Description = "Vivienda unifamiliar" },
                    new PropertyType { Name = "Apartamento", Description = "Unidad residencial en edificio" },
                    new PropertyType { Name = "Local Comercial", Description = "Espacio para negocios" },
                    new PropertyType { Name = "Terreno", Description = "Propiedad sin construcción" },
                    new PropertyType { Name = "Oficina", Description = "Espacio de trabajo profesional" }
                };

                await context.PropertyTypes.AddRangeAsync(propertyTypes);
                await context.SaveChangesAsync();
            }

            // Seed Sale Types
            if (!await context.SaleTypes.AnyAsync())
            {
                var saleTypes = new List<SaleType>
                {
                    new SaleType { Name = "Venta", Description = "Venta de propiedad" },
                    new SaleType { Name = "Alquiler", Description = "Alquiler de propiedad" },
                    new SaleType { Name = "Venta/Alquiler", Description = "Disponible para venta o alquiler" }
                };

                await context.SaleTypes.AddRangeAsync(saleTypes);
                await context.SaveChangesAsync();
            }

            // Seed Improvements
            if (!await context.Improvements.AnyAsync())
            {
                var improvements = new List<Improvement>
                {
                    new Improvement { Name = "Piscina", Description = "Piscina privada o comunitaria" },
                    new Improvement { Name = "Gimnasio", Description = "Área de ejercicio equipada" },
                    new Improvement { Name = "Parqueo", Description = "Espacio de estacionamiento" },
                    new Improvement { Name = "Jardín", Description = "Área verde privada" },
                    new Improvement { Name = "Terraza", Description = "Área exterior techada" },
                    new Improvement { Name = "Balcón", Description = "Área exterior con vista" },
                    new Improvement { Name = "Aire Acondicionado", Description = "Sistema de climatización" },
                    new Improvement { Name = "Calefacción", Description = "Sistema de calefacción" },
                    new Improvement { Name = "Seguridad 24/7", Description = "Vigilancia permanente" },
                    new Improvement { Name = "Amueblado", Description = "Incluye mobiliario" }
                };

                await context.Improvements.AddRangeAsync(improvements);
                await context.SaveChangesAsync();
            }
        }
    }
}
