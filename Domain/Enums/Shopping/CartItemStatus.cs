using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Shopping
{
    public enum CartItemStatus
    {
        [Display(Name = "اقدام لازم")]
        Pending = 1,

        [Display(Name = "در حال بررسی")]
        InProgress = 2,

        [Display(Name = "تکمیل شده")]
        Completed = 3,
    }
}
