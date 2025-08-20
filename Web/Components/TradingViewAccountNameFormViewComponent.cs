using Application.Services.Interfaces;
using Application.ViewModels.Cart.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class TradingViewAccountNameFormViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        private readonly ICartItemService _itemService;
        public TradingViewAccountNameFormViewComponent(ICartService cartService,
            ICartItemService itemService)
        {
            _cartService = cartService;
            _itemService = itemService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _itemService.IsAnyTradingViewProductInCart();
            if (result.IsSuccess)
            {
                var nameResult = await _cartService.ClientSideGetCurrentUserTradingViewAccountName();
                if (nameResult.IsFailure)
                    return View("TradingViewAccountNameForm", new ClientSideUpdateTradingViewAccountNameViewModel()
                    {
                        IsAllowed = true
                    });

                return View("TradingViewAccountNameForm", new ClientSideUpdateTradingViewAccountNameViewModel()
                {
                    IsAllowed = true,
                    TradingViewAccountName = nameResult.Value
                });
            }
            else
            {
                return View("TradingViewAccountNameForm", new ClientSideUpdateTradingViewAccountNameViewModel());
            }
        }
    }
}
