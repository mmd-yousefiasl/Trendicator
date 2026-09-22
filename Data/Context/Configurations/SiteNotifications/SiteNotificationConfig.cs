using Domain.Models.SiteNotifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.SiteNotifications
{
    public class SiteNotificationConfig : IEntityTypeConfiguration<SiteNotification>
    {
        public void Configure(EntityTypeBuilder<SiteNotification> builder)
        {
            builder.ToTable("SiteNotifications");

            builder.HasKey(n => n.Id);

            builder.HasIndex(n => new { n.ForUserId, n.IsRead });

            builder.Property(n => n.Content)
                .HasMaxLength(2500)
                .IsRequired();

            builder.Property(n => n.IsRead)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
