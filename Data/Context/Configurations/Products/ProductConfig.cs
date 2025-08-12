using Domain.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Products
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.BannerFileName).HasMaxLength(50);
            builder.Property(n => n.Title).HasMaxLength(100);
            builder.Property(n => n.Slug).HasMaxLength(200);

            builder.HasOne(n => n.Category)
                .WithMany(n => n.Products)
                .HasForeignKey(n => n.CategoryId);
        }
    }
}
