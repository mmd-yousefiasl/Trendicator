using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Banner
{
    public enum BannerTypeEnum
    {
        [Display(Name = "650*295")]
        Banner650x295,
        [Display(Name = "565*550")]
        Banner560x550,
        [Display(Name = "روش های پرداخت")]
        PaymentMethods,
    }
}
