using Domain.Models.Blogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Blogs
{
    public class BlogCommentConfig : IEntityTypeConfiguration<BlogComment>
    {
        public void Configure(EntityTypeBuilder<BlogComment> builder)
        {
            builder.ToTable("BlogComments");

            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(n => n.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Comment)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(n => n.UserIP)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(n => n.Reply)
                .IsRequired(false)
                .HasMaxLength(1000);

            builder.HasOne(n => n.Blog)
                .WithMany(n => n.Comments)
                .HasForeignKey(n => n.BlogId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.HasIndex(n => new { n.BlogId, n.IsPublished });
        }
    }
}
