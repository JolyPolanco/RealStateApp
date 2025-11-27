using RealStateApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealStateApp.Infraestructure.Persistence.EntitiesConfiguration
{
    public class SaleTypeEntityConfiguration : IEntityTypeConfiguration<SaleType>
    {
        public void Configure(EntityTypeBuilder<SaleType> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
        }
    }
}
