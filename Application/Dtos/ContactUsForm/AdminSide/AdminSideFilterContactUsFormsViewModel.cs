using Domain.Enums.ContactUs;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.ContactUsForm.AdminSide
{
    public class AdminSideFilterContactUsFormsViewModel : BasePaging<AdminSideContactUsFormViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "موضوع")]
        public ContactUsFormTypeEnum? Type { get; set; }
    }
}
