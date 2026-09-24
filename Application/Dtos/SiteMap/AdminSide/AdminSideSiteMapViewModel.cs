using Application.Dtos;
using Domain.Enums.SiteInfo;

namespace Application.Dtos.SiteMap.AdminSide
{
    public class AdminSideSiteMapViewModel : CommonDto
    {
        public string Url { get; set; }
        public ChangeFreqEnum ChangeFreq { get; set; }
        public decimal Priority { get; set; }
        public bool IsPublished { get; set; }
        public SiteMapTypeEnum Type { get; set; }
    }
}
