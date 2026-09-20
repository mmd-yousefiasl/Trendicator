using Domain.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Products
{
    public class ProductCategoryConfig : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable("ProductCategories");

            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.Title).HasMaxLength(100);
            builder.Property(n => n.Slug).HasMaxLength(200);

            builder.HasOne(n => n.ParentCategory)
                .WithMany(n => n.SubCategories)
                .HasForeignKey(n => n.ParentCategoryId);
        }
    }
}
