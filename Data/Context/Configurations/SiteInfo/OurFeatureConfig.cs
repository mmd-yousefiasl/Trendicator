using Domain.Models.SiteInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.SiteInfo
{
    public class OurFeatureConfig : IEntityTypeConfiguration<OurFeature>
    {
        public void Configure(EntityTypeBuilder<OurFeature> builder)
        {
            builder.ToTable("OurFeatures");

            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.Title).HasMaxLength(50);
            builder.Property(n => n.Description).HasMaxLength(100);
            builder.Property(n => n.FileName).HasMaxLength(50);
        }
    }
}
