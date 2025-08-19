using Application.Services.Interfaces;
using Application.ViewModels.Coupon.AdminSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageCoupons")]
    public class CouponController : BaseAdminController
    {
        #region Fields
        private readonly ICouponService _couponService;
        #endregion

        #region Ctor
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        #endregion

        [Permission("CouponsList")]
        [HttpGet("/admin/coupons-list")]
        public async Task<IActionResult> CouponsList(AdminSideFilterCouponsViewModel filter)
        {
            var result = await _couponService.AdminSideFilterCoupons(filter);
            if (result.IsFailure) return View(new AdminSideFilterCouponsViewModel());

            return View(result.Value);
        }

        [Permission("CreateNewCoupon")]
        [HttpGet("/admin/add-new-coupon")]
        public IActionResult CreateNewCoupon()
        {
            return View();
        }

        [Permission("CreateNewCoupon")]
        [HttpPost("/admin/add-new-coupon"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewCoupon(AdminSideCreateNewCouponViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _couponService.CreateNewCoupon(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(CouponsList));
        }

        [Permission("UpdateCoupon")]
        [HttpGet("/admin/edit-coupon")]
        public async Task<IActionResult> UpdateCoupon(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(CouponsList));
            }
            var result = await _couponService.GetCouponFouUpdate(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(CouponsList));
            }

            return View(result.Value);
        }

        [Permission("UpdateCoupon")]
        [HttpPost("/admin/edit-coupon"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCoupon(AdminSideUpdateCouponViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _couponService.UpdateCoupon(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(CouponsList));
        }

        [Permission("DeleteCoupon")]
        [HttpPost("/admin/remove-coupon")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(CouponsList));
            }
            var result = await _couponService.SoftDeleteCoupon(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(CouponsList));
        }
    }
}
