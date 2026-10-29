using Domain.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Products
{
    public class ProductFeatureConfig : IEntityTypeConfiguration<ProductFeature>
    {
        public void Configure(EntityTypeBuilder<ProductFeature> builder)
        {
            builder.ToTable("ProductFeatures");

            builder.HasKey(n => n.Id);

            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.Text)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasOne(n => n.Product)
                .WithMany(n => n.Features)
                .HasForeignKey(n => n.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);
        }
    }
}
