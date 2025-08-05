using Domain.Models.Faqs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Faqs
{
    public class FaqConfig : IEntityTypeConfiguration<Faq>
    {
        public void Configure(EntityTypeBuilder<Faq> builder)
        {
            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.Question).HasMaxLength(300);
            builder.Property(n => n.Answer).HasMaxLength(800);
        }
    }
}
