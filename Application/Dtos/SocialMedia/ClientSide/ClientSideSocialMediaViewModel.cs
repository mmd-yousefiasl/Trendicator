using Domain.Enums.SiteInfo;

namespace Application.Dtos.SocialMedia.ClientSide
{
    public class ClientSideSocialMediaViewModel
    {
        public string Url { get; set; }
        public SocialMediaTypeEnum Type { get; set; }
    }
}
