using Domain.Enums.SiteInfo;

namespace Domain.Models.SiteInfo
{
    public class SmsPanel : BaseEntity<int>
    {
        public string ApiKey { get; set; }
        public int PatternId { get; set; }
        public SmsPanelTypeEnum Type { get; set; }
        public bool IsActive { get; set; }
    }
}
