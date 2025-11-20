using Domain.Models.SiteInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.SiteInfo
{
    public class SiteMapConfig : IEntityTypeConfiguration<SiteMap>
    {
        public void Configure(EntityTypeBuilder<SiteMap> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Url).HasMaxLength(500);
        }
    }
}
