using Domain.Enums.Banner;

namespace Application.ViewModels.Banner.AdminSide
{
    public class AdminSideBannerViewModel : CommonViewModel
    {
        public string Title { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public BannerTypeEnum Type { get; set; }
    }
}
