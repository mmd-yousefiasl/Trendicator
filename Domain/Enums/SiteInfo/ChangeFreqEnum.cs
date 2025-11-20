using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.SiteInfo
{
    public enum ChangeFreqEnum
    {
        [Display(Name = "daily")]
        Daily = 1,
        [Display(Name = "weekly")]
        Weekly = 2,
        [Display(Name = "monthly")]
        Monthly = 3,
    }
}
