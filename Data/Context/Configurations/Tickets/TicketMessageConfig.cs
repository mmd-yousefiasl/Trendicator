using Domain.Models.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Tickets
{
    public class TicketMessageConfig : IEntityTypeConfiguration<TicketMessage>
    {
        public void Configure(EntityTypeBuilder<TicketMessage> builder)
        {
            builder.ToTable("TicketMessages");

            builder.HasKey(tm => tm.Id);

            builder.Property(tm => tm.Text)
                .HasMaxLength(1500)
                .IsRequired();

            builder.Property(tm => tm.AttachmentPath)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.HasOne(tm => tm.Ticket)
                .WithMany(t => t.Messages)
                .HasForeignKey(tm => tm.TicketId)
                .IsRequired(false);

            builder.HasIndex(tm => tm.TicketId);
        }
    }
}
