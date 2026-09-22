using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Role.AdminSide
{
    public class AdminSideUpdateRoleViewModel : CommonDto
    {
        [Display(Name = "عنوان")]
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Display(Name = "نام یکتا")]
        [Required]
        [MaxLength(100)]
        public string UniqueName { get; set; }

        [Display(Name = "دسترسی ها")]
        [Required(ErrorMessage = Messages.RequiredAccessForRoleError)]
        public List<int> PermissionIds { get; set; }
    }
}
