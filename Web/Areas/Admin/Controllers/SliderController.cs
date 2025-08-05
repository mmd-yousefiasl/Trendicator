using Application.Services.Interfaces;
using Application.ViewModels.Slider.AdminSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageSlider")]
    public class SliderController : BaseAdminController
    {
        #region Fields
        private readonly ISliderService _sliderService;
        #endregion

        #region Ctor
        public SliderController(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }
        #endregion

        [Permission("SlidersList")]
        [HttpGet("/admin/sliders-list")]
        public async Task<IActionResult> SlidersList(AdminSideFilterSlidersViewModel filter)
        {
            var result = await _sliderService.AdminSideFilterSldiers(filter);
            if (result.IsFailure) return View(new AdminSideFilterSlidersViewModel());

            return View(result.Value);
        }

        [Permission("CreateNewSlider")]
        [HttpGet("/admin/add-new-slider")]
        public IActionResult CreateNewSlider()
        {
            return View();
        }

        [Permission("CreateNewSlider")]
        [HttpPost("/admin/add-new-slider"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewSlider(AdminSideCreateNewSliderViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _sliderService.AdminSideCreateNewSlider(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(SlidersList));
        }

        [Permission("UpdateSlider")]
        [HttpGet("/admin/edit-slider")]
        public async Task<IActionResult> UpdateSlider(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(SlidersList));
            }
            var result = await _sliderService.AdminSideGetSliderForUpdate(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(SlidersList));
            }
            return View(result.Value);
        }

        [Permission("UpdateSlider")]
        [HttpPost("/admin/edit-slider"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSlider(AdminSideUpdateSliderViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _sliderService.AdminSideUpdateSlider(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(SlidersList));
        }

        [Permission("DeleteSlider")]
        [HttpPost("/admin/remove-slider"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSlider(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(SlidersList));
            }
            var result = await _sliderService.AdminSideDeleteSlider(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(SlidersList));
        }
    }
}
