using Application.Dtos.Product.ClientSide;
using Application.Services.Interfaces;
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

        [HttpGet("/products/{category?}")]
        public async Task<IActionResult> ProductsList(string? category, ClientSideFilterProductsViewModel filter)
        {
            filter.TakeEntity = 9;
            if (!string.IsNullOrWhiteSpace(category) && filter.Sort is null && filter.Page == 1)
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

            await _productService.ClientSideAddProductView(result.Value.Id);

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

            await _productService.ClientSideAddProductView(result.Value.Id);

            return View(result.Value);
        }

        [HttpPost("/product/add-comment"), ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(ClientSideAddProductCommentDto model)
        {
            if (!ModelState.IsValid || model.Stars < 1 || model.Stars > 5)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductDetails), new { id = model.ProductId });
            }

            var result = await _productService.ClientSideAddProductComment(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(ProductDetails), new { id = model.ProductId });
        }
    }
}