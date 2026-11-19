using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class UserPanelProductActionNoticeViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        public UserPanelProductActionNoticeViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _cartService.UserHaveAnyPendingProduct();

            return View("UserPanelProductActionNotice", result);
        }
    }
}
