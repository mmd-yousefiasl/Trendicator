using Domain.Models.ContactUs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.ContactUs
{
    public class ContactUsFormConfig : IEntityTypeConfiguration<ContactUsForm>
    {
        public void Configure(EntityTypeBuilder<ContactUsForm> builder)
        {
            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.FullName).HasMaxLength(200);
            builder.Property(n => n.Email).HasMaxLength(200);
            builder.Property(n => n.PhoneNumber).HasMaxLength(50);
            builder.Property(n => n.Subject).HasMaxLength(200);
            builder.Property(n => n.Message).HasMaxLength(1000);
        }
    }
}
