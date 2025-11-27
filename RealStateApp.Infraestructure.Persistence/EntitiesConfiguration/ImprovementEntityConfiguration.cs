using RealStateApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealStateApp.Infraestructure.Persistence.EntitiesConfiguration
{
    public class ImprovementEntityConfiguration : IEntityTypeConfiguration<Improvement>
    {
        public void Configure(EntityTypeBuilder<Improvement> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
        }
    }
}
