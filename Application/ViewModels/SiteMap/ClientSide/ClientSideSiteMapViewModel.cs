using Domain.Enums.SiteInfo;

namespace Application.ViewModels.SiteMap.ClientSide
{
    public class ClientSideSiteMapViewModel
    {
        public DateTime LastUpdateDate { get; set; }
        public string Url { get; set; }
        public ChangeFreqEnum ChangeFreq { get; set; }
        public decimal Priority { get; set; }
    }
}
