using Domain.Enums.Users;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.BalanceTransaction.AdminSide
{
    public class AdminSideFilterBalanceTransactionsViewModel : BasePaging<AdminSideBalanceTransactionViewModel>
    {
        public int UserId { get; set; }

        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "نوع تراکنش")]
        public BalanceTransactionTypeEnum? Type { get; set; }

        [Display(Name = "وضعیت تراکنش")]
        public bool? IsSuccess { get; set; }
    }
}
