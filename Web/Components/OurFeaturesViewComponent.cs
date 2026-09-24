using Application.Dtos.OurFeature.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class OurFeaturesViewComponent : ViewComponent
    {
        private readonly IOurFeaturesService _ourFeatureService;
        public OurFeaturesViewComponent(IOurFeaturesService ourFeatureService)
        {
            _ourFeatureService = ourFeatureService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _ourFeatureService.GetOurFeatures();
            if (result.IsFailure) return View("OurFeatures", new List<ClientSideOurFeatureViewModel>());

            return View("OurFeatures", result.Value);
        }
    }
}
