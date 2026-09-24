using Application.Dtos;
using Domain.Enums.SiteInfo;

namespace Application.Dtos.SmsData.AdminSide
{
    public class AdminSideSmsPanelDto : CommonDto
    {
        public SmsPanelTypeEnum Type { get; set; }
        public bool IsActive { get; set; }
    }
}
