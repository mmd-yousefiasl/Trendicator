using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.User.AdminSide
{
    public class AdminSideUserDetailsDto
    {
        [Display(Name = "نام")]
        public string? FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string? LastName { get; set; }

        [Display(Name = "شماره تلفن")]
        public string Mobile { get; set; }

        [Display(Name = "وضعیت حساب کاربری")]
        public bool IsUserBanned { get; set; }
    }
}
