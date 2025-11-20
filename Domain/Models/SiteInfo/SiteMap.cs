using Domain.Enums.SiteInfo;

namespace Domain.Models.SiteInfo
{
    public class SiteMap : AuditBaseEntity<int>
    {
        public string Url { get; set; }
        public ChangeFreqEnum ChagneFreq { get; set; }
        public decimal Priority { get; set; }
        public bool IsPublished { get; set; }
        public SiteMapTypeEnum Type { get; set; }
    }
}
