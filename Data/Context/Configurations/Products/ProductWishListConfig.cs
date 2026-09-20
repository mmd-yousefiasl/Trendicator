using Domain.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Products
{
    public class ProductWishListConfig : IEntityTypeConfiguration<ProductWishList>
    {
        public void Configure(EntityTypeBuilder<ProductWishList> builder)
        {
            builder.ToTable("ProductWishLists");

            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.UserIP).HasMaxLength(50);

            builder.HasOne(n => n.Product)
                .WithMany(n => n.WishList)
                .HasForeignKey(n => n.ProductId);
        }
    }
}
