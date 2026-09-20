using Domain.Models.Coupons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Coupons
{
    public class CouponUserMappingConfig : IEntityTypeConfiguration<CouponUserMapping>
    {
        public void Configure(EntityTypeBuilder<CouponUserMapping> builder)
        {
            builder.ToTable("CouponUserMappings");

            builder.HasKey(n => new { n.UserId, n.CouponId });

            builder.HasOne(n => n.User)
                .WithMany(n => n.Coupons)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.HasOne(n => n.Coupon)
                .WithMany(n => n.Users)
                .HasForeignKey(n => n.CouponId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        }
    }
}
