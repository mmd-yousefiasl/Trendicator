using Domain.Enums.SiteInfo;

namespace Application.ViewModels.SiteMap.AdminSide
{
    public class AdminSideSiteMapViewModel : CommonViewModel
    {
        public string Url { get; set; }
        public ChangeFreqEnum ChangeFreq { get; set; }
        public decimal Priority { get; set; }
        public bool IsPublished { get; set; }
        public SiteMapTypeEnum Type { get; set; }
    }
}
