using Application.Dtos.ProductFaq.AdminSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class ProductFaqsViewComponent : ViewComponent
    {
        private readonly IProductFaqService _faqService;
        public ProductFaqsViewComponent(IProductFaqService faqService)
        {
            _faqService = faqService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var result = await _faqService.AdminSideFilterProductFaqs(new AdminSideFilterProductFaqsViewModel() { ProductId = id, TakeEntity = 50 });
            if (result.IsFailure)
                return View("ProductFaqs", new AdminSideFilterProductFaqsViewModel() { ProductId = id });

            return View("ProductFaqs", result.Value);
        }
    }
}
