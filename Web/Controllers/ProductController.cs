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

        [HttpGet("/products/{category?}")]
        public async Task<IActionResult> ProductsList(string? category, ClientSideFilterProductsViewModel filter)
        {
            if (!string.IsNullOrWhiteSpace(category) && string.IsNullOrWhiteSpace(filter.Search) && filter.Sort is null)
            {
                ViewData["NoIndex"] = false;
            }
            else
            {
                ViewData["NoIndex"] = true;
            }

            filter.Category = category;
            var result = await _productService.ClientSideFilterProducts(filter);
            if (result.IsFailure) return View(new ClientSideFilterProductsViewModel());

            return View(result.Value);
        }

        [HttpGet("/p/{id}/{slug}")]
        public async Task<IActionResult> ProductDetails(int id, string slug)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductsList));
            }

            var result = await _productService.ClientSideGetProductDetails(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(ProductsList));
            }

            await _viewService.AddProductView(result.Value.Id);

            return View(result.Value);
        }

        [HttpGet("/p/{id}")]
        public async Task<IActionResult> ProductDetails(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductsList));
            }

            var result = await _productService.ClientSideGetProductDetails(id);
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
            ViewData["NoIndex"] = true;

            var result = await _productService.ClientSideGetProductDetails(slug);
            if (result.IsFailure) return PartialView("_ProductQuickView", new ClientSideProductDetailsViewModel());

            await _viewService.AddProductView(result.Value.Id);

            return PartialView("_ProductQuickView", result.Value);
        }
    }
}
