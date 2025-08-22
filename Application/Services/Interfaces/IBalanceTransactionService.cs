using Application.ViewModels.BalanceTransaction.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IBalanceTransactionService
    {
        Task<Result> AddUnVerifiedTransaction(ClientSideCreateNewTransactionViewModel model);

        Task<Result<long>> GetTransactionAmountByTransId(string transId);

        Task<Result> VerifyTransaction(string transId);
    }
}
