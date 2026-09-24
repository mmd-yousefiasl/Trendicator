using Application.Dtos.ProductOption.AdminSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class ProductOptionsViewComponent : ViewComponent
    {
        private readonly IProductOptionService _optionService;
        public ProductOptionsViewComponent(IProductOptionService optionService)
        {
            _optionService = optionService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var result = await _optionService.AdminSideFilterProductOptions(new AdminSideFilterProductOptionsViewModel() { ProductId = id, TakeEntity = 50 });
            if (result.IsFailure) return View("ProductOptions", new AdminSideFilterProductOptionsViewModel() { ProductId = id });

            return View("ProductOptions", result.Value);
        }
    }
}
