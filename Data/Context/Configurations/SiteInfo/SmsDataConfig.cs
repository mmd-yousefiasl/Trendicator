using Domain.Models.SiteInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.SiteInfo
{
    public class SmsDataConfig : IEntityTypeConfiguration<SmsData>
    {
        public void Configure(EntityTypeBuilder<SmsData> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.SmsPanelAPIKey).HasMaxLength(500);
        }
    }
}
