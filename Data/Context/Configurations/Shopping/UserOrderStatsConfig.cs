using Domain.Models.Shopping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Shopping
{
    public class UserOrderStatsConfig : IEntityTypeConfiguration<UserOrderStats>
    {
        public void Configure(EntityTypeBuilder<UserOrderStats> builder)
        {
            builder.ToTable("UserOrderStats");

            builder.HasKey(n => n.UserId);

            builder.Property(u => u.TotalPaid)
                .HasColumnType("numeric(18,0)");

            builder.Property(u => u.TotalDiscount)
           .HasColumnType("numeric(18,0)");
        }
    }
}
