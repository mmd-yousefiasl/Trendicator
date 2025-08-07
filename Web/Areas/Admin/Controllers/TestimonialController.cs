using Application.Services.Interfaces;
using Application.ViewModels.Testimonial.AdminSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageTestimonials")]
    public class TestimonialController : BaseAdminController
    {
        #region Fields
        private readonly ITestimonialService _testimonialService;
        #endregion

        #region Ctor
        public TestimonialController(ITestimonialService testimonialService)
        {
            _testimonialService = testimonialService;
        }
        #endregion

        [Permission("TestimonialsList")]
        [HttpGet("/admin/testimonials-list")]
        public async Task<IActionResult> TestimonialsList(AdminSideFilterTestimonialsViewModel filter)
        {
            var result = await _testimonialService.AdminSideFilterTestimonials(filter);
            if (result.IsFailure) return View(new AdminSideFilterTestimonialsViewModel());

            return View(result.Value);
        }

        [Permission("CreateNewTestimonial")]
        [HttpGet("/admin/add-new-testimonial")]
        public IActionResult CreateNewTestimonial()
        {
            return View();
        }

        [Permission("CreateNewTestimonial")]
        [HttpPost("/admin/add-new-testimonial"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewTestimonial(AdminSideCreateNewTestimonialViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _testimonialService.AdminSideCreateNewTestimonial(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(TestimonialsList));
        }

        [Permission("UpdateTestimonial")]
        [HttpGet("/admin/edit-testimonial")]
        public async Task<IActionResult> UpdateTestimonial(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(TestimonialsList));
            }
            var result = await _testimonialService.AdminSideGetTestimonialForUpdate(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(TestimonialsList));
            }

            return View(result.Value);
        }

        [Permission("UpdateTestimonial")]
        [HttpPost("/admin/edit-testimonial"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTestimonial(AdminSideUpdateTestimonialViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _testimonialService.AdminSideUpdateTestimonial(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(TestimonialsList));
        }

        [Permission("DeleteTestimonial")]
        [HttpPost("/admin/remove-testimonial"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTestimonial(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(TestimonialsList));
            }
            var result = await _testimonialService.AdminSideDeleteTestimonial(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;

            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(TestimonialsList));
        }
    }
}
