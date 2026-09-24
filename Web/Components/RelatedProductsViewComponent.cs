using Application.Dtos.Product.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class RelatedProductsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        public RelatedProductsViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id, int categoryId)
        {
            var result = await _productService.ClientSideGetRelatedProducts(id, categoryId);
            if (result.IsFailure) return View("RelatedProducts", new List<ClientSideProductViewModel>());

            return View("RelatedProducts", result.Value);
        }
    }
}
