using Domain.Models.Shopping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Shopping
{
    public class CartConfig : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.UserIP).HasMaxLength(50);
            builder.Property(n => n.DiscountCode).HasMaxLength(200);

            // Configure Id to start from 10000
            builder.Property(n => n.Id)
                   .UseIdentityColumn(seed: 10000, increment: 1);
        }
    }
}
