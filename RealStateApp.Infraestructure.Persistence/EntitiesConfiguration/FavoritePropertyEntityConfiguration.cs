using RealStateApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealStateApp.Infraestructure.Persistence.EntitiesConfiguration
{
    public class FavoritePropertyEntityConfiguration : IEntityTypeConfiguration<FavoriteProperty>
    {
        public void Configure(EntityTypeBuilder<FavoriteProperty> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(x => x.ClientId).IsRequired().HasMaxLength(450);

            // Configurar índice único compuesto para evitar duplicados
            builder.HasIndex(fp => new { fp.ClientId, fp.PropertyId }).IsUnique();

            // Configurar la relación con Property
            builder.HasOne(fp => fp.Property)
                   .WithMany(p => p.FavoriteProperties)
                   .HasForeignKey(fp => fp.PropertyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
