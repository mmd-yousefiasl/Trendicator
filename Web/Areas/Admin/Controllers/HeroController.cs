using Application.Dtos.Hero.AdminSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageHero")]
    public class HeroController : BaseAdminController
    {
        #region Fields
        private readonly IHeroService _heroService;
        #endregion

        #region Ctor
        public HeroController(IHeroService heroService)
        {
            _heroService = heroService;
        }
        #endregion

        [Permission("UpdateHero")]
        [HttpGet("/admin/edit-hero")]
        public async Task<IActionResult> UpdateHero()
        {
            var result = await _heroService.AdminSideGetHeroForUpdate();
            if (result.IsFailure) return View(new AdminSideUpdateHeroViewModel());

            return View(result.Value);
        }

        [Permission("UpdateHero")]
        [HttpPost("/admin/edit-hero"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateHero(AdminSideUpdateHeroViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _heroService.AdminSideUpdateHero(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(UpdateHero));
        }
    }
}
