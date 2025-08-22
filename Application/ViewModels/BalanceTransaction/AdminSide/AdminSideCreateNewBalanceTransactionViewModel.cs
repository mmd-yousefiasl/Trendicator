using Domain.Enums.Users;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.BalanceTransaction.AdminSide
{
    public class AdminSideCreateNewBalanceTransactionViewModel
    {
        [Display(Name = "مقدار")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, int.MaxValue, ErrorMessage = Messages.RangeError)]
        public long Amount { get; set; }

        [Display(Name = "نوع تراکنش")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public BalanceTransactionTypeEnum Type { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        public string Description { get; set; }

        [Display(Name = "کاربر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public int UserId { get; set; }
    }
}
