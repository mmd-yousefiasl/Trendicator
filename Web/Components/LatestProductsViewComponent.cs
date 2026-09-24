using Application.Dtos.Product.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class LatestProductsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        public LatestProductsViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _productService.ClientSideGetLatestProducts();
            if (result.IsFailure) return View("LatestProducts", new List<ClientSideProductViewModel>());

            return View("LatestProducts", result.Value);
        }
    }
}
