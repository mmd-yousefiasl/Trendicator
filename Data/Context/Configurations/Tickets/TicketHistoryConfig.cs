using Domain.Models.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Tickets
{
    public class TicketHistoryConfig : IEntityTypeConfiguration<TicketHistory>
    {
        public void Configure(EntityTypeBuilder<TicketHistory> builder)
        {
            builder.ToTable("TicketHistories");

            builder.HasKey(th => th.Id);

            builder.Property(th => th.Description)
                .HasMaxLength(1000)
                .IsRequired(true);

            builder.HasOne(th => th.Ticket)
                .WithMany(t => t.TicketHistories)
                .HasForeignKey(th => th.TicketId)
                .IsRequired(false);

            builder.HasIndex(th => th.TicketId);

            builder.HasIndex(th => th.ActorId);
        }
    }
}
