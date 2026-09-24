using Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.SiteData.AdminSide
{
    public class AdminSideUpdateSiteDataViewModel
    {
        [Display(Name = "نام سایت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string SiteName { get; set; }

        [Display(Name = "لوگو سایت")]
        public string? DarkBgLogoFileName { get; set; }

        [Display(Name = "لوگو سایت")]
        public IFormFile? DarkBgLogoFile { get; set; }

        [Display(Name = "لوگو سایت")]
        public string? LightBgLogoFileName { get; set; }

        [Display(Name = "لوگو سایت")]
        public IFormFile? LightBgLogoFile { get; set; }

        [Display(Name = "شماره تلفن")]
        [MaxLength(50, ErrorMessage = Messages.MaxLengthError)]
        public string? Phone { get; set; }

        [Display(Name = "توضیحات فوتر")]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        public string? FooterDescription { get; set; }

        [Display(Name = "آدرس")]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        public string? Address { get; set; }

        [Display(Name = "ایمیل")]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string? Email { get; set; }

        [Display(Name = "متن فوتر")]
        [MaxLength(1000, ErrorMessage = Messages.MaxLengthError)]
        public string? FooterContent { get; set; }

        [Display(Name = "شرایط و ضوابط")]
        [MaxLength(10000, ErrorMessage = Messages.MaxLengthError)]
        public string? TermsAndCondition { get; set; }

        [Display(Name = "سیاست حفظ حریم خصوصی")]
        [MaxLength(10000, ErrorMessage = Messages.MaxLengthError)]
        public string? PrivacyPolicy { get; set; }
    }
}
