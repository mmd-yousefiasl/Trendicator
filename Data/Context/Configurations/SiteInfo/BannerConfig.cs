using Domain.Models.SiteInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.SiteInfo
{
    public class BannerConfig : IEntityTypeConfiguration<Banner>
    {
        public void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.FileName).HasMaxLength(50);
            builder.Property(n => n.Title).HasMaxLength(100);
            builder.Property(n => n.Url).HasMaxLength(500);
        }
    }
}
