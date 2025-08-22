using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Payment
{
    public enum PaymentResultStatusEnum
    {
        [Display(Name = "ناموفق")]
        Failed = 0,
        [Display(Name = "موفق")]
        Succeed = 1
    }
}
