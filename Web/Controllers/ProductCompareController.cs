using Application.Services.Interfaces;
using Application.ViewModels.ProductCompare.ClientSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ProductCompareController : BaseController
    {
        #region Fields
        private readonly IProductCompareService _compareService;
        #endregion

        #region Ctor
        public ProductCompareController(IProductCompareService compareService)
        {
            _compareService = compareService;
        }
        #endregion

        [HttpGet("/product/compare")]
        public async Task<IActionResult> Compare(ClientSideFilterProductComareViewModel filter)
        {
            filter.TakeEntity = 4;
            var result = await _compareService.ClientSideFilterProductCompare(filter);
            if (result.IsFailure) return View(new ClientSideFilterProductComareViewModel());

            return View(result.Value);
        }

        [HttpPost("/product/toggle-compare")]
        public async Task<IActionResult> UpdateCompare([FromBody] ClientSideUpdateProductCompareViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            switch (model.Action)
            {
                case "Add":
                    await _compareService.AddToCompare(model.ProductId);
                    break;
                case "Remove":
                    await _compareService.RemoveFromCompare(model.ProductId);
                    break;
                default:
                    return BadRequest();
            }

            return Ok();
        }

        [HttpGet("/product/remove-compare")]
        public async Task<IActionResult> DeleteCompare(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(Compare));
            }
            var result = await _compareService.DeleteCompare(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(Compare));
        }
    }
}
