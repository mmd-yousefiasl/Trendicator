using Application.Dtos.BalanceTransaction.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Controllers
{
    public class MyTransactionsController : BaseUserPanelController
    {
        #region Fields
        private readonly IBalanceTransactionService _transactionService;
        #endregion

        #region  Ctor
        public MyTransactionsController(IBalanceTransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        #endregion

        [HttpGet("/user/my-transactions")]
        public async Task<IActionResult> MyTransactions(ClientSideFilterTransactionsViewModel filter)
        {
            var result = await _transactionService.ClientSideFilterTransaction(filter);
            if (result.IsFailure) return View(new ClientSideFilterTransactionsViewModel());

            return View(result.Value);
        }
    }
}
