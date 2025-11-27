using RealStateApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealStateApp.Infraestructure.Persistence.EntitiesConfiguration
{
    public class PropertyEntityConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(x => x.Code).IsRequired().HasMaxLength(6);
            builder.HasIndex(x => x.Code).IsUnique();
            
            builder.Property(x => x.Price).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.SizeInMeters).IsRequired();
            builder.Property(x => x.Bedrooms).IsRequired();
            builder.Property(x => x.Bathrooms).IsRequired();
            builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.AgentId).IsRequired().HasMaxLength(450);

            builder.HasOne(p => p.PropertyType)
                   .WithMany(pt => pt.Properties)
                   .HasForeignKey(p => p.PropertyTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.SaleType)
                   .WithMany(st => st.Properties)
                   .HasForeignKey(p => p.SaleTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
