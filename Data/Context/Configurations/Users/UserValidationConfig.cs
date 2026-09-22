using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Users
{
    public class UserValidationConfig : IEntityTypeConfiguration<UserValidation>
    {
        public void Configure(EntityTypeBuilder<UserValidation> builder)
        {
            builder.ToTable("UserValidation");

            builder.HasKey(uv => uv.Id);
            builder.HasQueryFilter(uv => !uv.IsDeleted);

            builder.Property(uv => uv.Mobile).HasMaxLength(11);

            builder.HasIndex(uv => uv.Mobile);
        }
    }
}
