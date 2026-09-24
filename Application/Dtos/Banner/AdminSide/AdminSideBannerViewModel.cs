using Application.Dtos;
using Domain.Enums.Banner;

namespace Application.Dtos.Banner.AdminSide
{
    public class AdminSideBannerViewModel : CommonDto
    {
        public string Title { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public BannerTypeEnum Type { get; set; }
    }
}
