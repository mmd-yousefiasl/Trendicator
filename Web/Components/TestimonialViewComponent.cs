using Application.Services.Interfaces;
using Application.ViewModels.Testimonial.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class TestimonialViewComponent : ViewComponent
    {
        private readonly ITestimonialService _testimonialService;
        public TestimonialViewComponent(ITestimonialService testimonialService)
        {
            _testimonialService = testimonialService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _testimonialService.ClientSideGetTestimonials();
            if (result.IsFailure) return View("Testimonial", new List<ClientSideTestimonialViewModel>());

            return View("Testimonial", result.Value);
        }
    }
}
