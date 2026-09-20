using Domain.Models.Blogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Blogs
{
    public class BlogViewConfig : IEntityTypeConfiguration<BlogView>
    {
        public void Configure(EntityTypeBuilder<BlogView> builder)
        {
            builder.ToTable("BlogViews");

            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.UserIP).HasMaxLength(50);

            builder.HasOne(n => n.Blog)
                .WithMany(n => n.Views)
                .HasForeignKey(n => n.BlogId);
        }
    }
}
