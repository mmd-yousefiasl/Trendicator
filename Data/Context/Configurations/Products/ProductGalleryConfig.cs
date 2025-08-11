using Domain.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Products
{
    public class ProductGalleryConfig : IEntityTypeConfiguration<ProductGallery>
    {
        public void Configure(EntityTypeBuilder<ProductGallery> builder)
        {
            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.FileName).HasMaxLength(50);

            builder.HasOne(n => n.Product)
                .WithMany(n => n.Gallery)
                .HasForeignKey(n => n.ProductId);
        }
    }
}
