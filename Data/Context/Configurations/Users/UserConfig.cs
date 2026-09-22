using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Users
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(n => n.Id);

            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(u => u.Mobile)
                .HasMaxLength(11)
                .IsRequired();

            builder.HasIndex(u => u.Mobile)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = 'false'");

            builder.Property(u => u.Balance)
                .HasColumnType("numeric(18,0)");

            builder.Property(n => n.Avatar).HasMaxLength(50);
            builder.Property(n => n.FirstName).HasMaxLength(200);
            builder.Property(n => n.LastName).HasMaxLength(200);
            builder.Property(n => n.Email).HasMaxLength(200);

            builder.HasOne(n => n.ParentUser)
                .WithMany(n => n.SubUsers)
                .HasForeignKey(n => n.ParentUserId);
        }
    }
}
