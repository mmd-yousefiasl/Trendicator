using Domain.Models.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Tickets
{
    public class TicketConfig : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Tickets");

            builder.HasQueryFilter(t => !t.IsDeleted);

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasIndex(t => t.UserId);
        }
    }
}
