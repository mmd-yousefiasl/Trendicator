using Application.Services.Interfaces;
using Application.ViewModels.Product.ClientSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ProductController : BaseController
    {
        #region Fields
        private readonly IProductService _productService;
        private readonly IProductViewService _viewService;
        #endregion

        #region Ctor
        public ProductController(IProductService productService,
            IProductViewService viewService)
        {
            _productService = productService;
            _viewService = viewService;
        }
        #endregion

        [HttpGet("/products")]
        public async Task<IActionResult> ProductsList(ClientSideFilterProductsViewModel filter)
        {
            var result = await _productService.ClientSideFilterProducts(filter);
            if (result.IsFailure) return View(new ClientSideFilterProductsViewModel());

            return View(result.Value);
        }

        [HttpGet("/product/{slug}")]
        public async Task<IActionResult> ProductDetails(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductsList));
            }

            var result = await _productService.ClientSideGetProductDetails(slug);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(ProductsList));
            }

            await _viewService.AddProductView(result.Value.Id);

            return View(result.Value);
        }

        [HttpGet("/product/quick-view/{slug}")]
        public async Task<IActionResult> ProductQuickView(string slug)
        {
            var result = await _productService.ClientSideGetProductDetails(slug);
            if (result.IsFailure) return PartialView("_ProductQuickView", new ClientSideProductDetailsViewModel());

            await _viewService.AddProductView(result.Value.Id);

            return PartialView("_ProductQuickView", result.Value);
        }
    }
}
