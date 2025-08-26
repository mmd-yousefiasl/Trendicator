using Domain.Models.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Tickets
{
    public class TicketMessageConfig : IEntityTypeConfiguration<TicketMessage>
    {
        public void Configure(EntityTypeBuilder<TicketMessage> builder)
        {
            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.HasOne(n => n.Ticket)
                .WithMany(n => n.Messages)
                .HasForeignKey(n => n.TicketId);

            builder.Property(n => n.Message).HasMaxLength(500);
        }
    }
}
