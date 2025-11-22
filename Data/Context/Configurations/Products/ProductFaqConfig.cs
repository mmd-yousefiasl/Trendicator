using Domain.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Products
{
    public class ProductFaqConfig : IEntityTypeConfiguration<ProductFaq>
    {
        public void Configure(EntityTypeBuilder<ProductFaq> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Question).HasMaxLength(200);
            builder.Property(n => n.Answer).HasMaxLength(500);

            builder.HasOne(n => n.Product)
                .WithMany(n => n.Faqs)
                .HasForeignKey(n => n.ProductId);
        }
    }
}
