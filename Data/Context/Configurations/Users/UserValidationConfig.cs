using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Users
{
    public class UserValidationConfig : IEntityTypeConfiguration<UserValidation>
    {
        public void Configure(EntityTypeBuilder<UserValidation> builder)
        {
            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.PhoneNumber).HasMaxLength(11);
        }
    }
}
