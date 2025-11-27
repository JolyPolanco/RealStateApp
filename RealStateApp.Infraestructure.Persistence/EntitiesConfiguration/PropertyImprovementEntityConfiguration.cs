using RealStateApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealStateApp.Infraestructure.Persistence.EntitiesConfiguration
{
    public class PropertyImprovementEntityConfiguration : IEntityTypeConfiguration<PropertyImprovement>
    {
        public void Configure(EntityTypeBuilder<PropertyImprovement> builder)
        {
            // Configurar clave compuesta
            builder.HasKey(pi => new { pi.PropertyId, pi.ImprovementId });

            // Configurar la relación con Property
            builder.HasOne(pi => pi.Property)
                   .WithMany(p => p.PropertyImprovements)
                   .HasForeignKey(pi => pi.PropertyId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configurar la relación con Improvement
            builder.HasOne(pi => pi.Improvement)
                   .WithMany(i => i.PropertyImprovements)
                   .HasForeignKey(pi => pi.ImprovementId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
