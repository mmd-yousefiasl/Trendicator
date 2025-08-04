using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Users
{
    public class UserRoleMappingConfig : IEntityTypeConfiguration<UserRoleMapping>
    {
        public void Configure(EntityTypeBuilder<UserRoleMapping> builder)
        {
            builder.HasKey(n => new { n.UserId, n.RoleId });

            builder.HasOne(n => n.User)
                .WithMany(n => n.UserRoleMapping)
                .HasForeignKey(n => n.UserId);
        }
    }
}
