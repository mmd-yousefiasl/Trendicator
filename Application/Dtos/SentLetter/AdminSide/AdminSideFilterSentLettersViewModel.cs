using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.SentLetter.AdminSide
{
    public class AdminSideFilterSentLettersViewModel : BasePaging<AdminSideSentLetterViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت ارسال")]
        public bool? IsSucceed { get; set; }
    }
}
