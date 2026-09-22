using Domain.Enums.SiteInfo;

namespace Application.ViewModels.SmsData.AdminSide
{
    public class AdminSideSmsPanelDto : CommonDto
    {
        public SmsPanelTypeEnum Type { get; set; }
        public bool IsActive { get; set; }
    }
}
