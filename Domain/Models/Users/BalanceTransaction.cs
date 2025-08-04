using Domain.Enums.Users;

namespace Domain.Models.Users
{
    public class BalanceTransaction : AuditBaseEntity<int>
    {
        public decimal Amount { get; set; }
        public BalanceTransactionTypeEnum Type { get; set; }
        public string? Description { get; set; }
        public bool IsSuccess { get; set; }
        public string? TransId { get; set; }

        #region Relations
        public int UserId { get; set; }
        public User User { get; set; }
        #endregion
    }
}
