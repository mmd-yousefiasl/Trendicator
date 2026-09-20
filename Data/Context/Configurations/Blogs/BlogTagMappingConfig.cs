using Domain.Models.Blogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Blogs
{
    public class BlogTagMappingConfig : IEntityTypeConfiguration<BlogTagMapping>
    {
        public void Configure(EntityTypeBuilder<BlogTagMapping> builder)
        {
            builder.ToTable("BlogTagMappings");

            builder.HasKey(n => new { n.BlogId, n.TagId });

            builder.HasOne(n => n.Blog)
                .WithMany(n => n.Tags)
                .HasForeignKey(n => n.BlogId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.HasOne(n => n.Tag)
                .WithMany(n => n.Blogs)
                .HasForeignKey(n => n.TagId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        }
    }
}
