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
        public bool DeleteRequest { get; set; }
        public DateTime? DeleteRequestDate { get; set; }

        #region Relations
        public ICollection<UserRoleMapping>? UserRoleMapping { get; set; }
        public ICollection<BalanceTransaction>? Transactions { get; set; }
        #endregion
    }
}
