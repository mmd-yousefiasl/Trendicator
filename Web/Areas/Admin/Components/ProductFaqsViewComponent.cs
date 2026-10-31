using Application.Dtos.ProductFaq.AdminSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class ProductFaqsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        public ProductFaqsViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var result = await _productService.AdminSideFilterProductFaqs(new AdminSideFilterProductFaqsViewModel() { ProductId = id, TakeEntity = 50 });
            if (result.IsFailure)
                return View("ProductFaqs", new AdminSideFilterProductFaqsViewModel() { ProductId = id });

            return View("ProductFaqs", result.Value);
        }
    }
}
