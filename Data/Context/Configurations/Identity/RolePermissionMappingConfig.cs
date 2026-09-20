using Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Identity
{
    public class RolePermissionMappingConfig : IEntityTypeConfiguration<RolePermissionMapping>
    {
        public void Configure(EntityTypeBuilder<RolePermissionMapping> builder)
        {
            builder.ToTable("RolePermissionMappings");

            builder.HasKey(n => new { n.RoleId, n.PermissionId });

            builder.HasOne(n => n.Role)
                .WithMany(n => n.RolePermissionMapping)
                .HasForeignKey(n => n.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.HasOne(n => n.Permission)
                .WithMany(n => n.RolePermissionMapping)
                .HasForeignKey(n => n.PermissionId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        }
    }
}
