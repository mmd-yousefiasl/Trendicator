using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class SoldProductsCountViewComponent : ViewComponent
    {
        private readonly ICartItemService _itemService;
        public SoldProductsCountViewComponent(ICartItemService itemService)
        {
            _itemService = itemService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _itemService.SoldProductsCount();

            return View("SoldProductsCount", result);
        }
    }
}
