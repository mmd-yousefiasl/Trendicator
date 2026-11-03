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
            var result = await _productService.ClientSideFilterProducts(new ClientSideFilterProductsViewModel()
            {
                TakeEntity = 4,
                Sort = Domain.Enums.Products.ProductSortTypeEnum.Popularity,
                Category = null
            });
            if (result.IsFailure) return View("LatestProducts", new ClientSideFilterProductsViewModel());

            return View("LatestProducts", result.Value);
        }
    }
}