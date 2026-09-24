using Application.Dtos.Product.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class TopRatedProductsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        public TopRatedProductsViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _productService.ClientSideGetTopRatedProducts();
            if (result.IsFailure) return View("TopRatedProducts", new List<ClientSideProductViewModel>());

            return View("TopRatedProducts", result.Value);
        }
    }
}
