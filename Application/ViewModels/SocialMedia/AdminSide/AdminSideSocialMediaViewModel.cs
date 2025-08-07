using Domain.Enums.SiteInfo;

namespace Application.ViewModels.SocialMedia.AdminSide
{
    public class AdminSideSocialMediaViewModel : CommonViewModel
    {
        public string Url { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public SocialMediaTypeEnum Type { get; set; }
    }
}
