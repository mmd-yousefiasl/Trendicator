using Domain.Enums.SiteInfo;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.SmsData.AdminSide
{
    public class AdminSideFilterSmsPanelsDto : BasePaging<AdminSideSmsPanelDto>
    {
        [Display(Name = "نوع")]
        public SmsPanelTypeEnum? Type { get; set; }

        [Display(Name = "وضعیت")]
        public bool? IsActive { get; set; }
    }
}
