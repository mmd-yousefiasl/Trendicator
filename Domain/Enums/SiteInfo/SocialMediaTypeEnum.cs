using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.SiteInfo
{
    public enum SocialMediaTypeEnum
    {
        [Display(Name = "fa-facebook-f")]
        Facebook = 1,
        [Display(Name = "fa-twitter")]
        Twitter = 2,
        [Display(Name = "fa-linkedin")]
        Linkedin = 3,
        [Display(Name = "fa-youtube")]
        Youtube = 4,
        [Display(Name = "fa-telegram")]
        Telegram = 5,
        [Display(Name = "fa-instagram")]
        Instagram = 6,
    }
}
