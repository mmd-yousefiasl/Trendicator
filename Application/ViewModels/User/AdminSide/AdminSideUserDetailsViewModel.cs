using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User.AdminSide
{
    public class AdminSideUserDetailsViewModel
    {
        public string? Avatar { get; set; }

        [Display(Name = "نام")]
        public string? FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string? LastName { get; set; }

        [Display(Name = "ایمیل")]
        public string? Email { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "شماره تلفن")]
        public string PhoneNumber { get; set; }

        [Display(Name = "وضعیت حساب کاربری")]
        public bool IsUserBanned { get; set; }
    }
}
