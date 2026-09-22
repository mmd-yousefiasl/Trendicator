using Domain.Enums.Users;

namespace Application.ViewModels.BalanceTransaction.AdminSide
{
    public class AdminSideBalanceTransactionViewModel : CommonDto
    {
        public DateTime CreatedDate { get; set; }
        public long Amount { get; set; }
        public BalanceTransactionTypeEnum Type { get; set; }
        public string? Description { get; set; }
        public bool IsSuccess { get; set; }
        public string? TransId { get; set; }
    }
}
