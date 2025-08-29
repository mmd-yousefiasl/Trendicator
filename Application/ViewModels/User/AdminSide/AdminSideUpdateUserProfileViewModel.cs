using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User.AdminSide
{
    public class AdminSideUpdateUserProfileViewModel
    {
        public string? AvatarFileName { get; set; }
        public IFormFile? AvatarFile { get; set; }

        [Display(Name = "نام")]
        [MaxLength(200)]
        public string? FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MaxLength(200)]
        public string? LastName { get; set; }

        [Display(Name = "ایمیل")]
        [MaxLength(200)]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDate { get; set; }
    }
}
