using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ContactUsForm.AdminSide
{
    public class AdminSideFilterContactUsFormsViewModel : BasePaging<AdminSideContactUsFormViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }
    }
}
