using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.User.AdminSide
{
    public class AdminSideUpdateUserProfileDto
    {
        [Display(Name = "نام", Prompt = "نام شما")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی", Prompt = "نام خانوادگی شما")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string LastName { get; set; }

        public bool DeleteRequest { get; set; }
        public DateTime? DeleteRequestDate { get; set; }

        [Display(Name = "شماره موبایل")]
        public string? Mobile { get; set; }
    }
}
