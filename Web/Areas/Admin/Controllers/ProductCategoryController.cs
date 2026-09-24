using Application.Dtos.ProductCategory.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageProductCategories")]
    public class ProductCategoryController : BaseAdminController
    {
        #region Fields
        private readonly IProductCategoryService _categoryService;
        #endregion

        #region Ctor
        public ProductCategoryController(IProductCategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        #endregion

        [Permission("ProductCategoriesList")]
        [HttpGet("/admin/product-categories-list")]
        public async Task<IActionResult> ProductCategoriesList(AdminSideFilterProductCategoriesViewMdoel filter)
        {
            var result = await _categoryService.AdminSideFilterProductCategories(filter);
            if (result.IsFailure) return View(new AdminSideFilterProductCategoriesViewMdoel());

            return View(result.Value);
        }

        [Permission("CreateNewProductCategory")]
        [HttpGet("/admin/add-new-product-category")]
        public IActionResult CreateNewProductCategory()
        {
            return View();
        }

        [Permission("CreateNewProductCategory")]
        [HttpPost("/admin/add-new-product-category"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewProductCategory(AdminSideCreateNewProductCategoryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _categoryService.AdminSideCreateNewProductCategory(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(ProductCategoriesList));
        }

        [Permission("UpdateProductCategory")]
        [HttpGet("/admin/edit-product-category")]
        public async Task<IActionResult> UpdateProductCategory(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductCategoriesList));
            }
            var result = await _categoryService.AdminSideGetCategoryForUpdate(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(ProductCategoriesList));
            }

            return View(result.Value);
        }

        [Permission("UpdateProductCategory")]
        [HttpPost("/admin/edit-product-category"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProductCategory(AdminSideUpdateProductCategoryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _categoryService.AdminSideUpdateProductCategory(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(ProductCategoriesList));
        }

        [Permission("DeleteProductCategory")]
        [HttpPost("/admin/remove-product-category"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(ProductCategoriesList));
            }
            var result = await _categoryService.AdminSideDeleteProductCategory(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(ProductCategoriesList));
        }
    }
}
