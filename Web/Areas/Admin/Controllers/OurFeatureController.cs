using Application.Dtos.OurFeature.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageOurFeatures")]
    public class OurFeatureController : BaseAdminController
    {
        #region Fields
        private readonly IOurFeaturesService _featureService;
        #endregion

        #region Ctor
        public OurFeatureController(IOurFeaturesService featureService)
        {
            _featureService = featureService;
        }
        #endregion

        [Permission("OurFeaturesList")]
        [HttpGet("/admin/our-features-list")]
        public async Task<IActionResult> OurFeaturesList(AdminSideFilterOurFeaturesViewModel filter)
        {
            var result = await _featureService.FilterOurFeatures(filter);
            if (result.IsFailure) return View(new AdminSideFilterOurFeaturesViewModel());

            return View(result.Value);
        }

        [Permission("CreateNewOurFeature")]
        [HttpGet("/admin/add-our-feature")]
        public IActionResult CreateNewOurFeature()
        {
            return View();
        }

        [Permission("CreateNewOurFeature")]
        [HttpPost("/admin/add-our-feature"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewOurFeature(AdminSideCreateNewOurFeatureViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _featureService.CreateNewOurFeature(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(OurFeaturesList));
        }

        [Permission("UpdateOurFeature")]
        [HttpGet("/admin/edit-our-feature")]
        public async Task<IActionResult> UpdateOurFeature(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(OurFeaturesList));
            }

            var result = await _featureService.GetOurFeatureForUpdate(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(OurFeaturesList));
            }

            return View(result.Value);
        }

        [Permission("UpdateOurFeature")]
        [HttpPost("/admin/edit-our-feature"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOurFeature(AdminSideUpdateOurFeatureViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _featureService.UpdateOurFeature(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(OurFeaturesList));
        }

        [Permission("DeleteOurFeature")]
        [HttpPost("/admin/remove-our-feature"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOurFeature(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(OurFeaturesList));
            }
            var result = await _featureService.SoftDeleteOurFeature(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(OurFeaturesList));
        }
    }
}
