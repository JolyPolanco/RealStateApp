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
                Console.WriteLine("✅ PropertyTypes seeded successfully");
            }
            else
            {
                Console.WriteLine("ℹ️  PropertyTypes already exist, skipping seed");
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
                Console.WriteLine("✅ SaleTypes seeded successfully");
            }
            else
            {
                Console.WriteLine("ℹ️  SaleTypes already exist, skipping seed");
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
                Console.WriteLine("✅ Improvements seeded successfully");
            }
            else
            {
                Console.WriteLine("ℹ️  Improvements already exist, skipping seed");
            }
        }

        /// <summary>
        /// Force re-seed (deletes existing data and inserts new data)
        /// ⚠️ WARNING: This will delete all existing PropertyTypes, SaleTypes, and Improvements!
        /// </summary>
        public static async Task ForceReseedAsync(RealStateContext context)
        {
            Console.WriteLine("⚠️  FORCE RESEED: Deleting existing data...");
            
            // Delete existing data
            context.PropertyTypes.RemoveRange(await context.PropertyTypes.ToListAsync());
            context.SaleTypes.RemoveRange(await context.SaleTypes.ToListAsync());
            context.Improvements.RemoveRange(await context.Improvements.ToListAsync());
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Existing data deleted");
            
            // Re-seed
            await SeedAsync(context);
        }
    }
}
