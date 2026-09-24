using Application.Dtos.Product.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class MostPurchasedProductsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        public MostPurchasedProductsViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _productService.ClientSideGetMostPurchasedProducts();
            if (result.IsFailure) return View("MostPurchasedProducts", new List<ClientSideProductViewModel>());
            return View("MostPurchasedProducts", result.Value);
        }
    }
}
