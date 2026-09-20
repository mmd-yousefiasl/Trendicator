using Domain.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Products
{
    public class ProductCompareConfig : IEntityTypeConfiguration<ProductCompare>
    {
        public void Configure(EntityTypeBuilder<ProductCompare> builder)
        {
            builder.ToTable("ProductCompare");

            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.UserIP).HasMaxLength(50);

            builder.HasOne(n => n.Product)
                .WithMany(n => n.Compare)
                .HasForeignKey(n => n.ProductId);
        }
    }
}
