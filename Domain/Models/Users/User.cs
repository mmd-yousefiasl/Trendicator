using Domain.Models.Coupons;

namespace Domain.Models.Users
{
    public class User : AuditBaseEntity<int>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Mobile { get; set; }
        public bool IsUserBanned { get; set; }
        public decimal Balance { get; set; }
        public int AccessFailedCount { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }

        #region Relations
        public ICollection<UserRoleMapping>? UserRoleMapping { get; set; }
        public ICollection<BalanceTransaction>? Transactions { get; set; }
        public ICollection<CouponUserMapping>? Coupons { get; set; }
        #endregion
    }
}
