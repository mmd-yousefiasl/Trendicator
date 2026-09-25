using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.ContactUs
{
    public enum ContactUsFormTypeEnum
    {
        [Display(Name = "پیش خرید")]
        Puchase = 1,
        [Display(Name = "پشتیبانی فنی")]
        TechSupport = 2,
        [Display(Name = "مالی")]
        Financial = 3
    }
}
