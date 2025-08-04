using Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Identity
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(n => n.Id);

            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.Title).HasMaxLength(100);
            builder.Property(n => n.UniqueName).HasMaxLength(100);
        }
    }
}
