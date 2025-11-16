using Domain.Models.SiteInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.SiteInfo
{
    public class HeroConfig : IEntityTypeConfiguration<Hero>
    {
        public void Configure(EntityTypeBuilder<Hero> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title).HasMaxLength(100);
            builder.Property(n => n.Description).HasMaxLength(200);
            builder.Property(n => n.FileName).HasMaxLength(50);
        }
    }
}
