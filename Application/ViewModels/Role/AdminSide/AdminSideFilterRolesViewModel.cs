using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Role.AdminSide
{
    public class AdminSideFilterRolesViewModel : BasePaging<AdminSideRoleViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }
    }
}
