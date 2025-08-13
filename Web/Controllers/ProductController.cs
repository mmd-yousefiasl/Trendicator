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
        #endregion

        #region Ctor
        public ProductController(IProductService productService)
        {
            _productService = productService;
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

            return View(result.Value);
        }
    }
}
