using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.ProductFaq.AdminSide;
using Data.Context;
using Domain.Models.Products;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class ProductFaqService : IProductFaqService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<ProductFaq> _dbSet;
        #endregion

        #region Ctor
        public ProductFaqService(TrendicatorDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<ProductFaq>();
        }
        #endregion

        public async Task<Result> AdminSideCreateNewProductFaq(AdminSideCreateNewProductFaqViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var faq = new ProductFaq()
            {
                Question = model.Question,
                Answer = model.Answer,
                Priority = model.Priority,
                IsPublished = model.IsPublished,
                ProductId = model.ProductId
            };

            await _dbSet.AddAsync(faq);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteProductFaq(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var faq = await _dbSet.FindAsync(id);
            if (faq is null) return Result.Failure(Messages.OperationFailedError);

            _dbSet.Remove(faq);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterProductFaqsViewModel>> AdminSideFilterProductFaqs(AdminSideFilterProductFaqsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterProductFaqsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<ProductFaq>();

            conditions.Add(n => n.ProductId == filter.ProductId);

            IQueryable<ProductFaq> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<ProductFaq>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideProductFaqViewModel()
            {
                Id = n.Id,
                Question = n.Question,
                Answer = n.Answer,
                Priority = n.Priority,
                IsPublished = n.IsPublished,
                ProductId = n.ProductId
            }));

            return filter;
        }

        public async Task<Result<AdminSideUpdateProductFaqViewModel>> AdminSideGetProductFaqForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateProductFaqViewModel>(Messages.OperationFailedError);

            var faq = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUpdateProductFaqViewModel()
            {
                Id = n.Id,
                Question = n.Question,
                Answer = n.Answer,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
                ProductId = n.ProductId
            }).AsNoTracking().FirstOrDefaultAsync();
            if (faq is null) return Result.Failure<AdminSideUpdateProductFaqViewModel>(Messages.OperationFailedError);

            return faq;
        }

        public async Task<Result> AdminSideUpdateProductFaq(AdminSideUpdateProductFaqViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var faq = await _dbSet.FindAsync(model.Id);
            if (faq is null) return Result.Failure(Messages.OperationFailedError);

            faq.Question = model.Question;
            faq.Answer = model.Answer;
            faq.Priority = model.Priority;
            faq.IsPublished = model.IsPublished;

            _dbSet.Update(faq);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
