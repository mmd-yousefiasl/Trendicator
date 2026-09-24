using Application.Dtos.BalanceTransaction.AdminSide;
using Application.Dtos.BalanceTransaction.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IBalanceTransactionService
    {
        Task<Result> AddUnVerifiedTransaction(ClientSideCreateNewTransactionViewModel model);

        Task<Result<decimal>> GetTransactionAmountByTransId(string transId);

        Task<Result> VerifyTransaction(string transId);

        Task<Result<AdminSideFilterBalanceTransactionsViewModel>> AdminSideFilterTransactions(AdminSideFilterBalanceTransactionsViewModel filter);

        Task<Result<int>> AdminSideDeleteTransaction(int id);

        Task<Result> AdminSideCreateNewTransaction(AdminSideCreateNewBalanceTransactionViewModel model);

        Task<Result<ClientSideFilterTransactionsViewModel>> ClientSideFilterTransaction(ClientSideFilterTransactionsViewModel filter);
    }
}
