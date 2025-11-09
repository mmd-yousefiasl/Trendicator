using Application.Services.Interfaces;
using Application.ViewModels.Product.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class MostViewedProductsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        public MostViewedProductsViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _productService.ClientSideGetMostViewedProducts();
            if (result.IsFailure) return View("MostViewedProducts", new List<ClientSideProductViewModel>());

            return View("MostViewedProducts", result.Value);
        }
    }
}
