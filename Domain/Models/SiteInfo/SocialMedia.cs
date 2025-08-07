using Domain.Enums.SiteInfo;

namespace Domain.Models.SiteInfo
{
    public class SocialMedia : BaseEntity<int>
    {
        public string Url { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public SocialMediaTypeEnum Type { get; set; }
    }
}
