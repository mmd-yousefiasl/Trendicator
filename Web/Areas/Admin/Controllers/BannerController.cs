using Application.Dtos.Banner.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageBanners")]
    public class BannerController : BaseAdminController
    {
        #region Fields
        private readonly IBannerService _bannerService;
        #endregion

        #region Ctor
        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }
        #endregion


        [Permission("BannersList")]
        [HttpGet("/admin/banners-list")]
        public async Task<IActionResult> BannersList(AdminSideFilterBannersViewModel filter)
        {
            var result = await _bannerService.AdminSideFilterBanners(filter);
            if (result.IsFailure) return View(new AdminSideFilterBannersViewModel());

            return View(result.Value);
        }

        [Permission("CreateNewBanner")]
        [HttpGet("/admin/add-new-banner")]
        public IActionResult CreateNewBanner()
        {
            return View();
        }

        [Permission("CreateNewBanner")]
        [HttpPost("/admin/add-new-banner"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewBanner(AdminSideCreateNewBannerViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _bannerService.AdminSideCreateNewBanner(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(BannersList));
        }
         
        [Permission("UpdateBanner")]
        [HttpGet("/admin/edit-banner")]
        public async Task<IActionResult> UpdateBanner(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(BannersList));
            }
            var result = await _bannerService.AdminSideGetBannerForUpdate(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(BannersList));
            }
            return View(result.Value);
        }

        [Permission("UpdateBanner")]
        [HttpPost("/admin/edit-banner"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBanner(AdminSideUpdateBannerViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _bannerService.AdminSideUpdateBanner(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(BannersList));
        }

        [Permission("DeleteBanner")]
        [HttpPost("/admin/remove-banner"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(BannersList));
            }
            var result = await _bannerService.AdminSideDeleteBanner(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(BannersList));
        }
    }
}
