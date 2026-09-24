using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.User.AdminSide
{
    public class AdminSideUpdateUserProfileDto
    {
        [Display(Name = "نام")]
        [MaxLength(200)]
        public string? FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MaxLength(200)]
        public string? LastName { get; set; }
    }
}
