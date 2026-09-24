using Application.Dtos.SocialMedia.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class FooterSocialMediaViewComponent : ViewComponent
    {
        private readonly ISocialMediaService _mediaSerivce;
        public FooterSocialMediaViewComponent(ISocialMediaService mediaService)
        {
            _mediaSerivce = mediaService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _mediaSerivce.GetSocialMedia();
            if (result.IsFailure) return View("FooterSocialMedia", new List<ClientSideSocialMediaViewModel>());

            return View("FooterSocialMedia", result.Value);
        }
    }
}
