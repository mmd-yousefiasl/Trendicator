using Domain.Models.Shopping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Shopping
{
    public class CartConfig : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");

            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.UserIP).HasMaxLength(50);
            builder.Property(n => n.DiscountCode).HasMaxLength(200);
            builder.Property(n => n.TradingViewAccountName).HasMaxLength(500);

            builder.Property(n => n.Id)
                   .UseIdentityColumn()
                   .HasIdentityOptions(startValue: 10000, incrementBy: 1);
        }
    }
}
