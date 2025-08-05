using Application.Services.Interfaces;
using Application.ViewModels.Slider;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class SliderViewComponent : ViewComponent
    {
        private readonly ISliderService _sliderService;
        public SliderViewComponent(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _sliderService.ClientSideGetMainPageSliders();
            if (result.IsFailure) return View("Slider", new List<ClientSideSliderViewModel>());

            return View("Slider", result.Value);
        }
    }
}
