using Application.Dtos.BalanceTransaction.AdminSide;
using Application.Dtos.BalanceTransaction.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Enums.Users;
using Domain.Models.Users;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Result<decimal>> GetTransactionAmountByTransId(string transId)
        {
            if (string.IsNullOrWhiteSpace(transId)) return Result.Failure<decimal>(Messages.OperationFailedError);

            var amount = await _dbSet.Where(n => n.TransId == transId).Select(n => n.Amount).FirstOrDefaultAsync();
            if (amount < 1) return Result.Failure<decimal>(Messages.OperationFailedError);

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

        public async Task<Result> AdminSideCreateNewTransaction(AdminSideCreateNewBalanceTransactionViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var user = await _context.Set<User>().FindAsync(model.UserId);
            if (user is null) return Result.Failure(Messages.OperationFailedError);

            // Create the transaction
            var transaction = new BalanceTransaction()
            {
                Amount = model.Amount,
                Type = model.Type,
                Description = model.Description,
                UserId = model.UserId,
                IsSuccess = true,
            };

            // Update user's balance
            if (model.Type == BalanceTransactionTypeEnum.Credit) user.Balance += model.Amount;
            else user.Balance -= model.Amount;

            if (user.Balance < 0)
                return Result.Failure(Messages.OperationFailedError);

            // Save both transaction and updated user
            await _dbSet.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<int>> AdminSideDeleteTransaction(int id)
        {
            if (id < 1) return Result.Failure<int>(Messages.OperationFailedError);

            var transaction = await _dbSet.FindAsync(id);
            if (transaction is null) return Result.Failure<int>(Messages.OperationFailedError);

            transaction.IsDeleted = true;
            transaction.LastUpdatedDate = DateTime.UtcNow;

            _dbSet.Update(transaction);
            await _context.SaveChangesAsync();

            return transaction.UserId;
        }

        public async Task<Result<AdminSideFilterBalanceTransactionsViewModel>> AdminSideFilterTransactions(AdminSideFilterBalanceTransactionsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterBalanceTransactionsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<BalanceTransaction>();

            conditions.Add(n => n.UserId == filter.UserId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.TransId.Contains(filter.Search) ||
                n.Description.Contains(filter.Search));
            }

            if (filter.Type is not null)
            {
                conditions.Add(n => n.Type == filter.Type);
            }

            if (filter.IsSuccess is not null)
            {
                conditions.Add(n => n.IsSuccess == filter.IsSuccess);
            }

            IQueryable<BalanceTransaction> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<BalanceTransaction>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideBalanceTransactionViewModel()
            {
                Id = n.Id,
                Amount = n.Amount,
                Description = n.Description,
                IsSuccess = n.IsSuccess,
                TransId = n.TransId,
                Type = n.Type,
                CreatedDate = n.CreatedDate,
            }));

            return filter;
        }

        public async Task<Result<ClientSideFilterTransactionsViewModel>> ClientSideFilterTransaction(ClientSideFilterTransactionsViewModel filter)
        {
            if (filter is null) return Result.Failure<ClientSideFilterTransactionsViewModel>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<ClientSideFilterTransactionsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<BalanceTransaction>();

            conditions.Add(n => n.UserId == userId);

            IQueryable<BalanceTransaction> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<BalanceTransaction>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideTransactionViewModel()
            {
                Amount = n.Amount,
                IsSuccess = n.IsSuccess,
                Description = n.Description,
                TransId = n.TransId,
                Type = n.Type,
                DateTime = n.CreatedDate
            }));

            return filter;
        }
    }
}
