using Domain.Models.Shopping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Shopping
{
    public class CartItemConfig : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.HasOne(n => n.Cart)
                .WithMany(n => n.Items)
                .HasForeignKey(n => n.CartId);

            builder.HasOne(n => n.Product)
                .WithMany(n => n.CartItems)
                .HasForeignKey(n => n.ProductId);
        }
    }
}
