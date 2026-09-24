using Application.Dtos.Product.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageProducts")]
    public class ProductController : BaseAdminController
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

        [Permission("ProductsList")]
        [HttpGet("/admin/products-list")]
        public async Task<IActionResult> ProductsList(AdminSideFilterProductsViewModel filter)
        {
            var result = await _productService.AdminSideFilterProducts(filter);
            if (result.IsFailure) return View(new AdminSideFilterProductsViewModel());

            return View(result.Value);
        }

        [Permission("CreateNewProduct")]
        [HttpGet("/admin/add-new-product")]
        public IActionResult CreateNewProduct()
        {
            return View();
        }

        [Permission("CreateNewProduct")]
        [HttpPost("/admin/add-new-product"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewProduct(AdminSideCreateNewProductViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _productService.AdminSideCreateNewProduct(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(ProductsList));
        }

        [Permission("UpdateProduct")]
        [HttpGet("/admin/edit-product")]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductsList));
            }
            var result = await _productService.AdminSideGetProductForUpdate(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(ProductsList));
            }

            return View(result.Value);
        }

        [Permission("UpdateProduct")]
        [HttpPost("/admin/edit-product"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(AdminSideUpdateProductViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _productService.AdminSideUpdateProduct(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(ProductsList));
        }

        [Permission("DeleteProduct")]
        [HttpPost("/admin/remove-product"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductsList));
            }
            var result = await _productService.AdminSideDeleteProduct(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(ProductsList));
        }
    }
}
