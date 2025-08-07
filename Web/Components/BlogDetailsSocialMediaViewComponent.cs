using Application.Services.Interfaces;
using Application.ViewModels.SocialMedia.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class BlogDetailsSocialMediaViewComponent : ViewComponent
    {
        private readonly ISocialMediaService _mediaService;
        public BlogDetailsSocialMediaViewComponent(ISocialMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _mediaService.GetSocialMedia();
            if (result.IsFailure) return View("BlogDetailsSocialMedia", new List<ClientSideSocialMediaViewModel>());

            return View("BlogDetailsSocialMedia", result.Value);
        }
    }
}
