using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Context.Configurations.Users
{
    public class BalanceTransactionConfig : IEntityTypeConfiguration<BalanceTransaction>
    {
        public void Configure(EntityTypeBuilder<BalanceTransaction> builder)
        {
            builder.HasKey(n => n.Id);
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.Property(n => n.Description).HasMaxLength(500);
            builder.Property(n => n.TransId).HasMaxLength(200);

            builder.HasOne(n => n.User)
                .WithMany(n => n.Transactions)
                .HasForeignKey(n => n.UserId);
        }
    }
}
