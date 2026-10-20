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

            builder.HasIndex(n => new { n.BlogId, n.VisitorId })
                .IsUnique();

            builder.Property(n => n.UserIP)
                .HasMaxLength(50);

            builder.Property(n => n.VisitorId)
                .HasMaxLength(50);

            builder.HasOne(n => n.Blog)
                 .WithMany(n => n.Views)
                 .HasForeignKey(n => n.BlogId)
                 .OnDelete(DeleteBehavior.Cascade)
                 .IsRequired(false);
        }
    }
}
