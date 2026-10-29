using Domain.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Products
{
    public class ProductDetailsConfig : IEntityTypeConfiguration<ProductDetails>
    {
        public void Configure(EntityTypeBuilder<ProductDetails> builder)
        {
            builder.ToTable("ProductDetails");

            builder.HasKey(n => n.Id);

            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(n => n.Description)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasOne(n => n.Product)
                .WithMany(n => n.Details)
                .HasForeignKey(n => n.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);
        }
    }
}
