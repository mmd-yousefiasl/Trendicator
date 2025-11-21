using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.ProductReview.AdminSide;
using Application.ViewModels.ProductReview.ClientSide;
using Data.Context;
using Domain.Events.Products.ProductReview;
using Domain.Models.Products;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class ProductReviewService : IProductReviewService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<ProductReview> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public ProductReviewService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<ProductReview>();
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }
        #endregion

        public async Task<Result<AdminSideFilterProductReviewsViewModel>> AdminSideFilterProductReviews(AdminSideFilterProductReviewsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterProductReviewsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<ProductReview>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Product.Title.Contains(filter.Search) ||
                n.Review.Contains(filter.Search));
            }

            if (filter.Stars is not null)
            {
                conditions.Add(n => n.Stars == filter.Stars);
            }

            if (filter.IsConfirmed is not null)
            {
                conditions.Add(n => n.IsConfirmed == filter.IsConfirmed);
            }

            IQueryable<ProductReview> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<ProductReview>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideProductReviewViewModel()
            {
                Id = n.Id,
                IsConfirmed = n.IsConfirmed,
                FullName = n.UserName,
                Stars = n.Stars,
                ProductTitle = n.Product.Title,
                ProductId = n.ProductId,
                Review = n.Review,
                UserId = n.UserId,
                UserIP = n.UserIP,
                PhoneNumber = n.PhoneNumber,
            }));

            return filter;
        }

        public async Task<Result> ConfirmReview(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var review = await _dbSet.FindAsync(id);
            if (review is null) return Result.Failure(Messages.OperationFailedError);

            review.IsConfirmed = true;
            review.LastUpdatedDate = DateTime.Now;

            _dbSet.Update(review);
            await _context.SaveChangesAsync();

            ProductReviewUpdatedEvent ProductReviewUpdatedEvent = new ProductReviewUpdatedEvent(
            review.Id,
            review.UserName,
            review.Review,
            review.Stars,
            review.IsConfirmed,
            review.PhoneNumber,
            review.UserId,
            review.UserIP,
            review.ProductId);

            await _mediator.PublishEvent(ProductReviewUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> RejectReview(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var review = await _dbSet.FindAsync(id);
            if (review is null) return Result.Failure(Messages.OperationFailedError);

            review.IsConfirmed = false;
            review.LastUpdatedDate = DateTime.Now;

            _dbSet.Update(review);
            await _context.SaveChangesAsync();

            ProductReviewUpdatedEvent ProductReviewUpdatedEvent = new ProductReviewUpdatedEvent(
            review.Id,
            review.UserName,
            review.Review,
            review.Stars,
            review.IsConfirmed,
            review.PhoneNumber,
            review.UserId,
            review.UserIP,
            review.ProductId);

            await _mediator.PublishEvent(ProductReviewUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> SoftDeleteProductReview(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var review = await _dbSet.FindAsync(id);
            if (review is null) return Result.Failure(Messages.OperationFailedError);

            review.IsDeleted = true;
            review.LastUpdatedDate = DateTime.Now;

            _dbSet.Update(review);
            await _context.SaveChangesAsync();

            ProductReviewDeletedEvent ProductReviewDeletedEvent = new ProductReviewDeletedEvent(
                review.Id);

            await _mediator.PublishEvent(ProductReviewDeletedEvent);

            return Result.Success();
        }

        public async Task<Result> ClientSideWriteProductReview(ClientSideWriteProductReviewViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            ProductReview review = new ProductReview()
            {
                Review = model.Review,
                Stars = model.Stars,
                ProductId = model.ProductId,
                IsConfirmed = false,
                UserIP = userIP,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
            };

            if (userId > 0) review.UserId = userId;

            await _dbSet.AddAsync(review);
            await _context.SaveChangesAsync();

            ProductReviewCreatedEvent ProductReviewCreatedEvent = new ProductReviewCreatedEvent(
                review.UserName,
               review.Review,
               review.Stars,
               review.IsConfirmed,
               review.PhoneNumber,
               review.UserId,
               review.UserIP,
               review.ProductId);

            await _mediator.PublishEvent(ProductReviewCreatedEvent);

            return Result.Success();
        }

        public async Task<Result<ClientSideProductReviewRateDataViewModel>> GetProductReviewRate(int productId)
        {
            if (productId < 1) Result.Failure<ClientSideProductReviewRateDataViewModel>(Messages.OperationFailedError);

            var reviews = await _dbSet.Where(n => n.ProductId == productId && n.IsConfirmed).Select(n => n.Stars).ToListAsync();
            if (!reviews?.Any() ?? true) new ClientSideProductReviewRateDataViewModel();

            int sum = reviews.Select(n => (int)n).Sum();

            var rateData = new ClientSideProductReviewRateDataViewModel()
            {
                TotalCount = reviews.Count(),
                ProductId = productId
            };
            if (rateData.TotalCount < 1) return Result.Failure<ClientSideProductReviewRateDataViewModel>(Messages.OperationFailedError);
            rateData.Avrage = sum.GetRateAvrage(rateData.TotalCount);
            rateData.RoundedAvrage = rateData.Avrage.RoundToHigher();

            return rateData;
        }

        public async Task<Result<List<ClientSideProductReviewViewModel>>> ClientSideProductReviews(int productId)
        {
            if (productId < 1) return Result.Failure<List<ClientSideProductReviewViewModel>>(Messages.OperationFailedError);

            var reviews = await _dbSet.Where(n => n.ProductId == productId && n.IsConfirmed)
                .OrderByDescending(n => n.Id)
                .Select(n => new ClientSideProductReviewViewModel()
                {
                    CreatedDate = n.CreatedDate,
                    Rate = (int)n.Stars,
                    Review = n.Review,
                    FullName = n.UserName
                }).AsNoTracking().ToListAsync();
            if (!reviews?.Any() ?? true) return Result.Failure<List<ClientSideProductReviewViewModel>>(Messages.OperationFailedError);

            return reviews;
        }

        public async Task<Result<List<ClientSideProductReviewRateDataViewModel>>> GetProductsReviewRate(List<int> productIds)
        {
            if (!productIds?.Any() ?? true) return Result.Failure<List<ClientSideProductReviewRateDataViewModel>>(Messages.OperationFailedError);

            var reviews = await _dbSet.Where(n => productIds.Contains(n.ProductId) && n.IsConfirmed).AsNoTracking().ToListAsync();
            if (!reviews?.Any() ?? true) return Result.Failure<List<ClientSideProductReviewRateDataViewModel>>(Messages.OperationFailedError);

            var productReviews = new List<ClientSideProductReviewRateDataViewModel>();

            foreach (var productId in productIds ?? new List<int>())
            {
                int sum = reviews.Where(n => n.ProductId == productId).Select(n => (int)n.Stars).Sum();

                var rateData = new ClientSideProductReviewRateDataViewModel()
                {
                    TotalCount = reviews.Where(n => n.ProductId == productId).Count(),
                    ProductId = productId,
                };
                if (rateData.TotalCount > 0)
                {
                    rateData.Avrage = sum.GetRateAvrage(rateData.TotalCount);
                    rateData.RoundedAvrage = rateData.Avrage.RoundToHigher();

                    productReviews.Add(rateData);
                }
            }
            if (!productReviews.Any()) return Result.Failure<List<ClientSideProductReviewRateDataViewModel>>(Messages.OperationFailedError);

            return productReviews;
        }
    }
}
