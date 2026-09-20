using Domain.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Products
{
    public class ProductReviewConfig : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.ToTable("ProductReviews");

            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.Review).HasMaxLength(1000);
            builder.Property(n => n.UserName).HasMaxLength(100);
            builder.Property(n => n.PhoneNumber).HasMaxLength(11);
            builder.Property(n => n.UserIP).HasMaxLength(50);

            builder.HasOne(n => n.Product)
                .WithMany(n => n.Reviews)
                .HasForeignKey(n => n.ProductId);
        }
    }
}
