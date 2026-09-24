using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.User.AdminSide
{
    public class AdminSideCreateNewUserDto
    {
        [Display(Name = "نام")]
        [MaxLength(200)]
        public string? FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MaxLength(200)]
        public string? LastName { get; set; }

        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(11)]
        public string Mobile { get; set; }

        [Display(Name = "وضعیت حساب کاربری")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public bool IsUserBanned { get; set; }

        [Display(Name = "نقش")]
        public List<int>? RoleIds { get; set; }
    }
}
