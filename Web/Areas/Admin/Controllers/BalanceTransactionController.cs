using Application.Dtos.BalanceTransaction.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageBalanceTransaction")]
    public class BalanceTransactionController : BaseAdminController
    {
        #region Fields
        private readonly IBalanceTransactionService _transactionService;
        #endregion

        #region Ctor
        public BalanceTransactionController(IBalanceTransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        #endregion

        [Permission("BalanceTransactionsList")]
        [HttpGet("/admin/transactions-list")]
        public async Task<IActionResult> BalanceTransactionsList(AdminSideFilterBalanceTransactionsViewModel filter)
        {
            var result = await _transactionService.AdminSideFilterTransactions(filter);
            if (result.IsFailure) return View(new AdminSideFilterBalanceTransactionsViewModel() { UserId = filter.UserId });

            return View(result.Value);
        }

        [Permission("CreateNewBalanceTransaction")]
        [HttpGet("/admin/add-new-transaction")]
        public IActionResult CreateNewBalanceTransaction(int userId)
        {
            return View(new AdminSideCreateNewBalanceTransactionViewModel() { UserId = userId });
        }

        [Permission("CreateNewBalanceTransaction")]
        [HttpPost("/admin/add-new-transaction"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewBalanceTransaction(AdminSideCreateNewBalanceTransactionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _transactionService.AdminSideCreateNewTransaction(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(BalanceTransactionsList), new { UserId = model.UserId });
        }

        [Permission("DeleteBalanceTransaction")]
        [HttpPost("/admin/remove-transaction"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBalanceTransaction(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(BalanceTransactionsList));
            }
            var result = await _transactionService.AdminSideDeleteTransaction(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(BalanceTransactionsList));
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(BalanceTransactionsList), new { userId = result.Value });
        }
    }
}
