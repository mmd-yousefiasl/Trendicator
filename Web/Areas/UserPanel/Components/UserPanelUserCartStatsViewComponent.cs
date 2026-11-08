using Application.Dtos.UserOrderStats.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class UserPanelUserCartStatsViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        public UserPanelUserCartStatsViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _cartService.ClientSideGetUserOrderStats();
            if (result.IsFailure)
                return View("UserPanelUserCartStats", new ClientSideUserOrderStatsDto());

            return View("UserPanelUserCartStats", result.Value);
        }
    }
}
