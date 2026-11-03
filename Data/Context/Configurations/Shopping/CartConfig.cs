using Domain.Enums.Shopping;
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

            builder.Property(n => n.DiscountCode)
                .HasMaxLength(200);

            builder.Property(n => n.VisitorId)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(n => n.UserId)
                .IsRequired(false);

            builder.Property(n => n.TotalPaid)
                .HasColumnType("numeric(18,0)");

            builder.Property(n => n.TotalDiscount)
                .HasColumnType("numeric(18,0)");

            builder.Property(n => n.Id)
                   .UseIdentityColumn()
                   .HasIdentityOptions(startValue: 10000, incrementBy: 1);

            var pending = $"\"Status\" = {(int)CartStatusEnum.PendingPayment}";

            builder.HasIndex(c => new { c.UserId, c.Status })
                .IsUnique()
                .HasFilter($"\"UserId\" IS NOT NULL AND {pending}");

            builder.HasIndex(c => new { c.VisitorId, c.Status })
                .IsUnique()
                .HasFilter($"\"VisitorId\" IS NOT NULL AND {pending}");
        }
    }
}
