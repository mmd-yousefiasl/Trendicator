using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Shopping
{
    public enum CartStatusEnum
    {
        [Display(Name = "درانتظار پرداخت")]
        PendingPayment = 1,
        [Display(Name = "پرداخت شده")]
        Paid = 2
    }
}
