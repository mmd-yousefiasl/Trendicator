using Domain.Models.Blogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Blogs
{
    public class BlogCategoryConfig : IEntityTypeConfiguration<BlogCategory>
    {
        public void Configure(EntityTypeBuilder<BlogCategory> builder)
        {
            builder.ToTable("BlogCategories");

            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.Title).HasMaxLength(100);
            builder.Property(n => n.Slug).HasMaxLength(200);
            builder.Property(n => n.MetaDescription).HasMaxLength(200);
        }
    }
}
