using Application.Dtos.ProductReview.AdminSide;
using Application.Dtos.ProductReview.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
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

        public async Task<Result<AdminSideFilterProductReviewsDto>> AdminSideFilterProductReviews(AdminSideFilterProductReviewsDto filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterProductReviewsDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<ProductReview>();

            if (filter.Stars is not null)
            {
                conditions.Add(n => n.Stars == filter.Stars);
            }

            if (filter.IsPublished is not null)
            {
                conditions.Add(n => n.IsPublished == filter.IsPublished);
            }

            IQueryable<ProductReview> query = _context.ProductReviews;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<ProductReview>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideProductReviewDto()
            {
                Id = n.Id,
                Name = n.Name,
                Email = n.Email,
                Stars = n.Stars,
                Review = n.Review,
                IsPublished = n.IsPublished,
                ProductId = n.ProductId,
                ProductTitle = n.Product.Title,
                UserId = n.UserId,
                UserIP = n.UserIP,
            }));

            return filter;
        }

        public async Task<Result> ConfirmReview(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var review = await _dbSet.FindAsync(id);
            if (review is null) return Result.Failure(Messages.OperationFailedError);

            review.IsPublished = true;
            review.LastUpdatedDate = DateTime.UtcNow;

            _dbSet.Update(review);
            await _context.SaveChangesAsync();

            ProductReviewUpdatedEvent ProductReviewUpdatedEvent = new ProductReviewUpdatedEvent(
            review.Id,
            review.Name,
            review.Review,
            review.Stars,
            review.IsPublished,
            review.Email,
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

            review.IsPublished = false;
            review.LastUpdatedDate = DateTime.UtcNow;

            _dbSet.Update(review);
            await _context.SaveChangesAsync();

            ProductReviewUpdatedEvent ProductReviewUpdatedEvent = new ProductReviewUpdatedEvent(
            review.Id,
            review.Name,
            review.Review,
            review.Stars,
            review.IsPublished,
            review.Email,
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
            review.LastUpdatedDate = DateTime.UtcNow;

            _dbSet.Update(review);
            await _context.SaveChangesAsync();

            ProductReviewDeletedEvent ProductReviewDeletedEvent = new ProductReviewDeletedEvent(
                review.Id);

            await _mediator.PublishEvent(ProductReviewDeletedEvent);

            return Result.Success();
        }

        public async Task<Result<ClientSideProductReviewRateDataViewModel>> GetProductReviewRate(int productId)
        {
            if (productId < 1) Result.Failure<ClientSideProductReviewRateDataViewModel>(Messages.OperationFailedError);

            var reviews = await _dbSet.Where(n => n.ProductId == productId && n.IsPublished).Select(n => n.Stars).ToListAsync();
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

        public async Task<Result<List<ClientSideProductReviewDto>>> ClientSideProductReviews(int productId)
        {
            if (productId < 1) return Result.Failure<List<ClientSideProductReviewDto>>(Messages.OperationFailedError);

            var reviews = await _dbSet.Where(n => n.ProductId == productId && n.IsPublished)
                .OrderByDescending(n => n.Id)
                .Select(n => new ClientSideProductReviewDto()
                {
                    CreatedDate = n.CreatedDate,
                    Stars = n.Stars,
                    Review = n.Review,
                    Name = n.Name
                }).AsNoTracking().ToListAsync();
            if (!reviews?.Any() ?? true) return Result.Failure<List<ClientSideProductReviewDto>>(Messages.OperationFailedError);

            return reviews;
        }

        public async Task<Result<List<ClientSideProductReviewRateDataViewModel>>> GetProductsReviewRate(List<int> productIds)
        {
            if (!productIds?.Any() ?? true) return Result.Failure<List<ClientSideProductReviewRateDataViewModel>>(Messages.OperationFailedError);

            var reviews = await _dbSet.Where(n => productIds.Contains(n.ProductId) && n.IsPublished).AsNoTracking().ToListAsync();
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
