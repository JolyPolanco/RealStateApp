using RealStateApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealStateApp.Infraestructure.Persistence.EntitiesConfiguration
{
    public class PropertyPhotoEntityConfiguration : IEntityTypeConfiguration<PropertyPhoto>
    {
        public void Configure(EntityTypeBuilder<PropertyPhoto> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(x => x.ImageUrl).IsRequired().HasMaxLength(500);

            // Configurar la relación con Property
            builder.HasOne(pp => pp.Property)
                   .WithMany(p => p.Photos)
                   .HasForeignKey(pp => pp.PropertyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
