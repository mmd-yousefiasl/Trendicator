using Domain.Models.Shopping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Shopping
{
    public class CartItemConfig : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");

            builder.HasKey(n => new { n.CartId, n.ProductId });

            builder.Property(n => n.Title)
                .HasMaxLength(100);

            builder.Property(n => n.TradingViewAccountName)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(u => u.OrderPrice)
            .HasColumnType("numeric(18,0)");

            builder.Property(u => u.DisplayPrice)
                .HasColumnType("numeric(18,0)");

            builder.HasOne(n => n.Cart)
                .WithMany(n => n.Items)
                .HasForeignKey(n => n.CartId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);

            builder.HasOne(n => n.Product)
                .WithMany(n => n.CartItems)
                .HasForeignKey(n => n.ProductId)
                .IsRequired(false);
        }
    }
}
