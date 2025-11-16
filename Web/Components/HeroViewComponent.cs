using Application.Services.Interfaces;
using Application.ViewModels.Hero.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class HeroViewComponent : ViewComponent
    {
        private readonly IHeroService _heroService;
        public HeroViewComponent(IHeroService heroService)
        {
            _heroService = heroService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _heroService.ClientSideGetHero();
            if (result.IsFailure) return View("Hero", new ClientSideHeroViewModel());

            return View("Hero", result.Value);
        }
    }
}
