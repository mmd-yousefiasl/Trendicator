using Domain.Models.NewsLetters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.NewsLetters
{
    public class SentLetterConfig : IEntityTypeConfiguration<SentLetter>
    {
        public void Configure(EntityTypeBuilder<SentLetter> builder)
        {
            builder.ToTable("SentLetters");

            builder.HasKey(n => n.Id);

            builder.HasQueryFilter(n => !n.IsDeleted);
            builder.Property(n => n.Subject).HasMaxLength(100);
            builder.Property(n => n.Message).HasMaxLength(5000);
        }
    }
}
