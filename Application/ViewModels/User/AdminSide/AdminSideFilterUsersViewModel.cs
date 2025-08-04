using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User.AdminSide
{
    public class AdminSideFilterUsersViewModel : BasePaging<AdminSideUserViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت حساب کاربری")]
        public bool? IsUserBanned { get; set; }
    }
}
