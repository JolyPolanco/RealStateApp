using RealStateApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealStateApp.Infraestructure.Persistence.EntitiesConfiguration
{
    public class OfferEntityConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(x => x.Amount).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.ClientId).IsRequired().HasMaxLength(450);

            // Configurar la relación con Property
            builder.HasOne(o => o.Property)
                   .WithMany(p => p.Offers)
                   .HasForeignKey(o => o.PropertyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
