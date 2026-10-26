using Domain.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Products
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.BannerFileName).HasMaxLength(50);
            builder.Property(n => n.Title).HasMaxLength(100);
            builder.Property(n => n.Slug).HasMaxLength(200);
            builder.Property(n => n.ShortDescription).HasMaxLength(500);
            builder.Property(n => n.MetaTraderLink).HasMaxLength(200);
            builder.Property(n => n.IndicatorFileName).HasMaxLength(50);

            builder.Property(u => u.OrderPrice)
           .HasColumnType("numeric(18,0)");

            builder.Property(u => u.DisplayPrice)
           .HasColumnType("numeric(18,0)");

            builder.HasOne(n => n.Category)
                .WithMany(n => n.Products)
                .HasForeignKey(n => n.CategoryId);
        }
    }
}
