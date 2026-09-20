using Domain.Models.NewsLetters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.NewsLetters
{
    public class NewsLetterConfig : IEntityTypeConfiguration<NewsLetter>
    {
        public void Configure(EntityTypeBuilder<NewsLetter> builder)
        {
            builder.ToTable("NewsLetters");

            builder.HasKey(n => n.Id);

            builder.HasQueryFilter(n => !n.IsDeleted);
            builder.Property(n => n.Email).HasMaxLength(200);
            builder.Property(n => n.UserIP).HasMaxLength(50);
        }
    }
}
