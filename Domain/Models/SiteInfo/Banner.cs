using Domain.Enums.Banner;

namespace Domain.Models.SiteInfo
{
    public class Banner : BaseEntity<int>
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public BannerTypeEnum Type { get; set; }
    }
}
