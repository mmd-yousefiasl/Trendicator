using Domain.Models.SiteInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.SiteInfo
{
    public class SmsPanelConfig : IEntityTypeConfiguration<SmsPanel>
    {
        public void Configure(EntityTypeBuilder<SmsPanel> builder)
        {
            builder.ToTable("SmsPanels");

            builder.HasKey(sp => sp.Id);

            builder.HasQueryFilter(sp => !sp.IsDeleted);

            builder.Property(sp => sp.ApiKey)
                .HasMaxLength(200);
        }
    }
}
