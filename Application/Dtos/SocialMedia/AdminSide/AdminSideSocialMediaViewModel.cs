using Application.Dtos;
using Domain.Enums.SiteInfo;

namespace Application.Dtos.SocialMedia.AdminSide
{
    public class AdminSideSocialMediaViewModel : CommonDto
    {
        public string Url { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public SocialMediaTypeEnum Type { get; set; }
    }
}
