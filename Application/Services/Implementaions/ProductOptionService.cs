using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.ProductStock.AdminSide;
using Data.Context;
using Domain.Events.Products.ProductOption;
using Domain.Models.Products;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class ProductOptionService : IProductOptionService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<ProductOption> _dbSet;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public ProductOptionService(TrendicatorDbContext context,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<ProductOption>();
            _mediator = mediator;
        }
        #endregion

        public async Task<Result> AdminSideCreateNewProductOption(AdminSideCreateNewProductOptionViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var option = new ProductOption()
            {
                Title = model.Title,
                Price = model.Price,
                OrderPrice = model.OrderPrice,
                ProductId = model.ProductId
            };

            await _dbSet.AddAsync(option);
            await _context.SaveChangesAsync();

            ProductOptionCreatedEvent ProductOptionCreatedEvent = new ProductOptionCreatedEvent(
                option.Title,
                option.Price,
                option.OrderPrice);

            await _mediator.PublishEvent(ProductOptionCreatedEvent);

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteProductOption(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var option = await _dbSet.FindAsync(id);
            if (option is null) return Result.Failure(Messages.OperationFailedError);

            _dbSet.Remove(option);
            await _context.SaveChangesAsync();

            ProductOptionDeletedEvent ProductOptionDeletedEvent = new ProductOptionDeletedEvent(
                option.Id);

            await _mediator.PublishEvent(ProductOptionDeletedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterProductOptionsViewModel>> AdminSideFilterProductOptions(AdminSideFilterProductOptionsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterProductOptionsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<ProductOption>();
            conditions.Add(n => n.ProductId == filter.ProductId);

            IQueryable<ProductOption> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<ProductOption>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideProductOptionViewModel()
            {
                Id = n.Id,
                OrderPrice = n.OrderPrice,
                Price = n.Price,
                Title = n.Title
            }));

            return filter;
        }

        public async Task<Result<AdminSideUpdateProductOptionViewModel>> AdminSideGetProductOptionForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateProductOptionViewModel>(Messages.OperationFailedError);

            var option = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUpdateProductOptionViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                Price = n.Price,
                OrderPrice = n.OrderPrice,
                ProductId = n.ProductId
            }).AsNoTracking().FirstOrDefaultAsync();
            if (option is null) return Result.Failure<AdminSideUpdateProductOptionViewModel>(Messages.OperationFailedError);

            return option;
        }

        public async Task<Result> AdminSideUpdateProductOption(AdminSideUpdateProductOptionViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var option = await _dbSet.FindAsync(model.Id);
            if (option is null) return Result.Failure(Messages.OperationFailedError);

            option.Title = model.Title;
            option.Price = model.Price;
            option.OrderPrice = model.OrderPrice;

            _dbSet.Update(option);
            await _context.SaveChangesAsync();

            ProductOptionUpdatedEvent ProductOptionUpdatedEvent = new ProductOptionUpdatedEvent(
                option.Id,
                option.Title,
                option.Price,
                option.OrderPrice);

            await _mediator.PublishEvent(ProductOptionUpdatedEvent);

            return Result.Success();
        }
    }
}
