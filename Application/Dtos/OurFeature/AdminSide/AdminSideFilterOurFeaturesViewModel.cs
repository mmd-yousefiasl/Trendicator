using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.OurFeature.AdminSide
{
    public class AdminSideFilterOurFeaturesViewModel : BasePaging<AdminSideOurFeatureViewModel>
    {
        [Display(Name = "جست و جو")]
        public string? Search { get; set; }

        [Display(Name = "وضعیت انتشار")]
        public bool? IsPublished { get; set; }
    }
}
