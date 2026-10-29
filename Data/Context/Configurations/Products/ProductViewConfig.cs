using Domain.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Products
{
    public class ProductViewConfig : IEntityTypeConfiguration<ProductView>
    {
        public void Configure(EntityTypeBuilder<ProductView> builder)
        {
            builder.ToTable("ProductViews");

            builder.HasIndex(n => new { n.ProductId, n.VisitorId })
                .IsUnique();

            builder.Property(n => n.UserIP)
                .HasMaxLength(50);

            builder.Property(n => n.VisitorId)
                .HasMaxLength(50);

            builder.HasOne(n => n.Product)
                .WithMany(n => n.Views)
                .HasForeignKey(n => n.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                 .IsRequired(false);
        }
    }
}
