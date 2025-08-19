using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Shopping
{
    public enum CartStatusEnum
    {
        [Display(Name = "درانتظار پرداخت")]
        PendingPayment = 1,
        [Display(Name = "درحال بررسی")]
        UnderReview = 2,
        [Display(Name = "لفوشده")]
        Canceled = 3,
        [Display(Name = "درحال انجام")]
        InProgress = 4,
        [Display(Name = "تکمیل شده")]
        Done = 5
    }
}
