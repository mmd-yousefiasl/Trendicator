using Domain.Models.SiteInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.SiteInfo
{
    public class SiteDataConfig : IEntityTypeConfiguration<SiteData>
    {
        public void Configure(EntityTypeBuilder<SiteData> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.SiteName).HasMaxLength(200);
            builder.Property(n => n.DarkBgLogo).HasMaxLength(50);
            builder.Property(n => n.LightBgLogo).HasMaxLength(50);
            builder.Property(n => n.Phone).HasMaxLength(50);
            builder.Property(n => n.FooterDescription).HasMaxLength(500);
            builder.Property(n => n.Address).HasMaxLength(500);
            builder.Property(n => n.Email).HasMaxLength(200);
            builder.Property(n => n.TermsAndCondition).HasMaxLength(10000);
            builder.Property(n => n.PrivacyPolicy).HasMaxLength(10000);
            builder.Property(n => n.FooterContent).HasMaxLength(1000);
        }
    }
}
