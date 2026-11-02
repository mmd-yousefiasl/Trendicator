using Application.Dtos.Cart.ClientSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Web.Controllers
{
    public class CartController : BaseController
    {
        #region Fields
        private readonly ICartService _cartService;
        private readonly ICartItemService _itemService;
        private static readonly HashSet<int> InCart = new();
        #endregion

        #region Ctor
        public CartController(ICartService cartService,
            ICartItemService itemService)
        {
            _cartService = cartService;
            _itemService = itemService;
        }
        #endregion

        [HttpGet("/cart")]
        public async Task<IActionResult> Cart()
        {
            ViewData["NoIndex"] = true;
            var result = await _cartService.ClientSideGetCart();
            if (result.IsFailure) return View(new List<ClientSideCartViewModel>());

            return View(result.Value);
        }

        public class CartToggleRequest
        {
            public int ProductId { get; set; }
        }

        [HttpPost("/cart/add")]
        public async Task<IActionResult> Add([FromBody] CartToggleRequest req)
        {
            /*            if (req?.ProductId <= 0)
                            return Json(new { success = false, message = "شناسه محصول نامعتبر است." }); */
            await Task.Delay(500);

            InCart.Add(req.ProductId);

            return Json(new
            {
                success = true,
                inCart = true,
                message = "محصول به سبد خرید اضافه شد.",
                cartCount = InCart.Count
            });
        }

        [HttpPost("/cart/remove")]
        public async Task<IActionResult> Remove([FromBody] CartToggleRequest req)
        {
            await Task.Delay(500);
            if (req?.ProductId <= 0)
                return Json(new { success = false, message = "شناسه محصول نامعتبر است." });

            InCart.Remove(req.ProductId);

            return Json(new
            {
                success = true,
                inCart = false,
                message = "محصول از سبد خرید حذف شد.",
                cartCount = InCart.Count
            });
        }

        [HttpGet("/add-to-cart/{optionId}")]
        public async Task<IActionResult> AddToCart(int optionId)
        {
            if (optionId < 1) return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _cartService.AddToCart(optionId);
            if (result.IsFailure) return Json(new { success = false, message = result.Message });

            return Json(new { success = true, message = result.Message });
        }

        [HttpGet("/cart-modal")]
        public async Task<IActionResult> CartModal()
        {
            ViewData["NoIndex"] = true;
            var result = await _cartService.ClientSideGetCart();
            if (result.IsFailure) return PartialView("_CartModal", new List<ClientSideCartViewModel>());

            return PartialView("_CartModal", result.Value);
        }

        [HttpGet("/remove-cart-modal-item/{id}")]
        public async Task<IActionResult> RemoveCartModalItem(int id)
        {
            if (id < 1) return Json(new { success = false, message = Messages.OperationFailedError });

            var result = await _itemService.ClientSideDeleteItemFromCart(id);
            if (result.IsFailure) return Json(new { success = false, message = result.Message });

            return Json(new { success = true, message = result.Message });
        }

        [HttpGet("/cart/remove/{id}")]
        public async Task<IActionResult> RemoveCartItem(int id)
        {
            ViewData["NoIndex"] = true;
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(Cart));
            }
            var result = await _itemService.ClientSideDeleteItemFromCart(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(Cart));
        }

        public class ApplyCouponRequest
        {
            public string Code { get; set; }
        }

        [HttpPost("/cart/apply-coupon")]
        public IActionResult ApplyCoupon([FromBody] ApplyCouponRequest request)
        {

            var discount = 10000;
            var total = 20000;

            return Json(new { success = true, html = RenderSummary("Welcome20", 10, discount, total) });
        }

        [HttpPost("/cart/remove-coupon")]
        public IActionResult RemoveCoupon()
        {
            return Json(new { success = true, html = RenderSummary(null, 0, 0, 20000) });
        }

        #region Helpers
        // ─── Helpers ────────────────────────────────────────────────

        private static string Fa(decimal v) =>
            v.ToString("N0", new CultureInfo("fa-IR"));

        private static string Fa(int v) =>
            v.ToString("N0", new CultureInfo("fa-IR"));

        private static string RenderSummary(string code, int percent, decimal discount, decimal total)
        {
            var items = $"""
            <div class="flex flex--between" style="font-size:.9rem;color:var(--text-2)">
                <span>محصولات</span>
                <span>{Fa(3)} مورد</span>
            </div>
            """;

            var discountRow = discount > 0
                ? $"""
              <div class="flex flex--between mt-1" style="font-size:.9rem;color:var(--text-2)">
                  <span>تخفیف</span>
                  <span class="num text-brand">− {Fa(discount)} تومان</span>
              </div>
              """
                : "";

            var couponBlock = code is null
                ? IdleCouponBlock()
                : AppliedCouponBlock(code, percent);

            return $"""
            <h3>خلاصه سفارش</h3>

            {items}

            <div class="flex flex--between mt-1" style="font-size:.9rem;color:var(--text-2)">
                <span>جمع محصولات</span>
                <span class="num">{Fa(10000)} تومان</span>
            </div>

            {discountRow}

            <div class="coupon mt-1" data-coupon>
                {couponBlock}
            </div>

            <hr style="border:none;border-top:1px dashed var(--line);margin:10px 0">

            <div class="flex flex--between" style="font-weight:800">
                <span>مبلغ قابل پرداخت</span>
                <span class="price">
                    <small>تومان</small>
                    {Fa(total)}
                </span>
            </div>

            <a href="/checkout" class="btn btn--primary btn--block btn--lg mt-3" data-cart-pay>
                ادامه و پرداخت
            </a>

            <p class="hint mt-2 mb-0 text-center">
                <svg class="ic" aria-hidden="true"><use href="#i-shield-lock" /></svg>
                اتصال SSL · درگاه رسمی شتاب
            </p>
            """;
        }

        private static string IdleCouponBlock() => """
        <div data-coupon-idle>
            <div class="coupon__row">
                <span>کد تخفیف</span>
                <a href="#" class="coupon__toggle" data-open-coupon>افزودن کد</a>
            </div>
            <div class="coupon__expand" data-coupon-form>
                <div class="coupon__expand-inner">
                    <div class="coupon__input-row">
                        <input class="input coupon__input"
                               type="text"
                               placeholder="WELCOME20"
                               autocomplete="off"
                               dir="ltr"
                               data-coupon-input />
                        <button class="btn btn--primary btn--sm" type="button" data-coupon-apply>اعمال</button>
                    </div>
                    <div class="coupon__error" data-coupon-error hidden>
                        <svg viewBox="0 0 16 16" width="14" height="14"
                             fill="none" stroke="currentColor" stroke-width="1.8"
                             stroke-linecap="round" stroke-linejoin="round">
                            <circle cx="8" cy="8" r="6.5" />
                            <path d="M8 4.5v4M8 11.2v.1" />
                        </svg>
                        <span data-coupon-error-text></span>
                    </div>
                </div>
            </div>
        </div>
        """;

        private static string AppliedCouponBlock(string code, int percent) => $"""
        <div class="coupon-applied" data-coupon-applied aria-live="polite">
            <div class="coupon-applied__chip">
                <div class="coupon-applied__percent">{Fa(percent)}٪</div>
                <div class="coupon-applied__body">
                    <b>کد تخفیف اعمال شد</b>
                    <span class="coupon-applied__code">
                        <svg viewBox="0 0 16 16" width="12" height="12"
                             fill="none" stroke="currentColor" stroke-width="2"
                             stroke-linecap="round" stroke-linejoin="round">
                            <path d="M3 8.5l3.2 3.2L13 5" />
                        </svg>
                        {code}
                    </span>
                </div>
                <button type="button" class="coupon-applied__remove" aria-label="حذف کد تخفیف">
                    <svg viewBox="0 0 16 16" width="16" height="16"
                         fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round">
                        <path d="M3 3l10 10M13 3L3 13" />
                    </svg>
                </button>
            </div>
        </div>
        """;
        #endregion
    }
}