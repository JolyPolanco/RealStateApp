using RealStateApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealStateApp.Infraestructure.Persistence.EntitiesConfiguration
{
    public class MessageEntityConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(x => x.Content).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.SenderUserId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.ReceiverUserId).IsRequired().HasMaxLength(450);

            // Configurar la relación con Property
            builder.HasOne(m => m.Property)
                   .WithMany(p => p.Messages)
                   .HasForeignKey(m => m.PropertyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
