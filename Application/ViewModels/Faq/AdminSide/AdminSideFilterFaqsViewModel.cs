using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Faq.AdminSide
{
    public class AdminSideFilterFaqsViewModel : BasePaging<AdminSideFaqViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsPublished { get; set; }
    }
}
