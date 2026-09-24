using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.User.AdminSide
{
    public class AdminSideFilterUsersDto : BasePaging<AdminSideUserDto>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت حساب کاربری")]
        public bool? IsUserBanned { get; set; }
    }
}
