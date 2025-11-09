using Application.Services.Interfaces;
using Application.ViewModels.Product.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class MainPageDiscountedProductsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        public MainPageDiscountedProductsViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _productService.ClientSideGetDiscountedProducts();
            if (result.IsFailure) return View("MainPageDiscountedProducts", new List<ClientSideProductViewModel>());

            return View("MainPageDiscountedProducts", result.Value);
        }
    }
}
