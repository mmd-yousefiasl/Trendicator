using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.NewsLetter.AdminSide
{
    public class AdminSideFilterNewsLettersViewModel : BasePaging<AdminSideNewsLetterViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }
    }
}
