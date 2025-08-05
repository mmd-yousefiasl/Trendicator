using Domain.Models.Blogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Blogs
{
    public class BlogConfig : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.FileName).HasMaxLength(50);
            builder.Property(n => n.Title).HasMaxLength(200);
            builder.Property(n => n.Slug).HasMaxLength(400);
            builder.Property(n => n.Writer).HasMaxLength(100);

            builder.HasOne(n => n.Category)
                .WithMany(n => n.Blogs)
                .HasForeignKey(n => n.BlogCategoryId);
        }
    }
}
