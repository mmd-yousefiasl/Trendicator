namespace Domain.Models.Users
{
    public class User : AuditBaseEntity<int>
    {
        public string? Avatar { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsUserBanned { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }

        #region Relations
        public ICollection<UserRoleMapping>? UserRoleMapping { get; set; }
        public ICollection<BalanceTransaction>? Transactions { get; set; }
        public ICollection<User>? SubUsers { get; set; }
        public int? ParentUserId { get; set; }
        public User? ParentUser { get; set; }
        #endregion
    }
}
