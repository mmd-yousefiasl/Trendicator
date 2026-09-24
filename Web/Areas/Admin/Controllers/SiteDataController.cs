using Application.Dtos.SiteData.AdminSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageSiteData")]
    public class SiteDataController : BaseAdminController
    {
        #region Fields
        private readonly ISiteDataService _dataService;
        #endregion

        #region Ctor
        public SiteDataController(ISiteDataService dataService)
        {
            _dataService = dataService;
        }
        #endregion

        [Permission("UpdateSiteData")]
        [HttpGet("/admin/edit-site-data")]
        public async Task<IActionResult> UpdateSiteData()
        {
            var result = await _dataService.AdminSideGetSiteDataForUpdate();
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            return View(result.Value);
        }

        [Permission("UpdateSiteData")]
        [HttpPost("/admin/edit-site-data"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSiteData(AdminSideUpdateSiteDataViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _dataService.AdminSideUpdateSiteData(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(UpdateSiteData));
        }
    }
}
