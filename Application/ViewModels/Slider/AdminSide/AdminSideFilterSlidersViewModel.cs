using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Slider.AdminSide
{
    public class AdminSideFilterSlidersViewModel : BasePaging<AdminSideSliderViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsPublished { get; set; }
    }
}
