using Application.Dtos.CartItem.ClientSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Controllers
{
    public class DownloadsController : BaseUserPanelController
    {
        #region Fields
        private readonly ICartItemService _itemService;
        #endregion

        #region Ctor
        public DownloadsController(ICartItemService itemService)
        {
            _itemService = itemService;
        }
        #endregion

        [HttpGet("/user/bought-files")]
        public async Task<IActionResult> BoughtFiles(ClientSideFilterBoughtFilesViewModel filter)
        {
            var result = await _itemService.ClientSideFilterBoughtFiles(filter);
            if (result.IsFailure) return View(new ClientSideFilterBoughtFilesViewModel());

            return View(result.Value);
        }

        [HttpGet("/user/download-file/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(BoughtFiles));
            }
            //check if can user downlaod file and returns file name is succeed
            var result = await _itemService.CanUserDownlaodTheCurrentFile(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(BoughtFiles));
            }

            if (!System.IO.File.Exists(result.Value.FileAddress))
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(BoughtFiles));
            }
            var stream = new FileStream(result.Value.FileAddress, FileMode.Open, FileAccess.Read);
            return File(stream, "application/zip", result.Value.FileName + ".zip");
        }
    }
}
