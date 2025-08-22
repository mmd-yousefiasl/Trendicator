using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.BalanceTransaction.ClientSide;
using Data.Context;
using Domain.Models.Users;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Application.Services.Implementaions
{
    public class BalanceTransactionService : IBalanceTransactionService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private readonly DbSet<BalanceTransaction> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public BalanceTransactionService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<BalanceTransaction>();
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }
        #endregion

        public async Task<Result> AddUnVerifiedTransaction(ClientSideCreateNewTransactionViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            var transaction = new BalanceTransaction()
            {
                Amount = model.Amount,
                Type = model.Type,
                Description = model.Description,
                IsSuccess = model.IsSuccess,
                TransId = model.TransId,
                UserId = userId
            };

            await _dbSet.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<long>> GetTransactionAmountByTransId(string transId)
        {
            if (string.IsNullOrWhiteSpace(transId)) return Result.Failure<long>(Messages.OperationFailedError);

            var amount = await _dbSet.Where(n => n.TransId == transId).Select(n => n.Amount).FirstOrDefaultAsync();
            if (amount < 1) return Result.Failure<long>(Messages.OperationFailedError);

            return amount;
        }

        public async Task<Result> VerifyTransaction(string transId)
        {
            var transaction = await _dbSet
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.TransId == transId);

            if (transaction is null)
                return Result.Failure(Messages.OperationFailedError);

            if (transaction.IsSuccess)
                return Result.Failure(Messages.TransactionAlreadyVerified);

            transaction.IsSuccess = true;
            transaction.User.Balance += transaction.Amount;

            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
