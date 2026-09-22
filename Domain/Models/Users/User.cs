using Domain.Models.Coupons;

namespace Domain.Models.Users
{
    public class User : AuditBaseEntity<int>
    {
        public string? Avatar { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Mobile { get; set; }
        public bool IsUserBanned { get; set; }
        public long Balance { get; set; }
        public int AccessFailedCount { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }

        #region Relations

        public ICollection<UserRoleMapping>? UserRoleMapping { get; set; }
        public ICollection<BalanceTransaction>? Transactions { get; set; }
        public ICollection<User>? SubUsers { get; set; }
        public ICollection<CouponUserMapping>? Coupons { get; set; }

        public int? ParentUserId { get; set; }
        public User? ParentUser { get; set; }

        #endregion
    }
}
