using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.Product.AdminSide;
using Application.ViewModels.Product.ClientSide;
using Data.Context;
using Domain.Enums.Products;
using Domain.Events.Products.Product;
using Domain.Models.Products;
using Domain.Shared;
using Domain.Statics;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class ProductService : IProductService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Product> _dbSet;
        private readonly IMediatorHandler _mediator;
        private readonly IProductWishListService _wishListService;
        private readonly IProductCompareService _compareService;
        private readonly IProductReviewService _reviewService;
        #endregion

        #region Ctor
        public ProductService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            IProductWishListService wishListService,
            IProductCompareService compareService,
            IProductReviewService reviewService)
        {
            _context = context;
            _dbSet = _context.Set<Product>();
            _mediator = mediator;
            _wishListService = wishListService;
            _compareService = compareService;
            _reviewService = reviewService;
        }
        #endregion

        public async Task<Result> AdminSideCreateNewProduct(AdminSideCreateNewProductViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);
            if (await _dbSet.AnyAsync(n => n.Slug == model.Slug.Replace(" ", "-"))) return Result.Failure(Messages.DuplicateSlugError);

            if (!model.BannerFile.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
            if (model.BannerFile.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.BannerFile.FileName);

            model.BannerFile.UplaodFileToServer(fileName, SiteTools.ProductBanner);

            var product = new Product()
            {
                BannerFileName = fileName,
                Title = model.Title,
                Slug = model.Slug.Replace(" ", "-"),
                Description = model.Description,
                DisplayPrice = model.DisplayPrice,
                DisplayOrderPrice = model.DisplayOrderPrice,
                IsAvailable = model.IsAvailable,
                IsPublished = model.IsPublished,
                Priority = model.Priority,
                CategoryId = model.CategoryId,
                ShortDescription = model.ShortDescription
            };

            await _dbSet.AddAsync(product);
            await _context.SaveChangesAsync();

            ProductCreatedEvent ProductCreatedEvent = new ProductCreatedEvent(
                product.BannerFileName,
                product.Title,
                product.Slug,
                product.ShortDescription,
                product.Description,
                product.Priority,
                product.IsPublished,
                product.DisplayPrice,
                product.DisplayOrderPrice,
                product.IsAvailable);

            await _mediator.PublishEvent(ProductCreatedEvent);

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteProduct(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var product = await _dbSet.Where(n => n.Id == id).Include(n => n.Gallery).FirstOrDefaultAsync();
            if (product is null) return Result.Failure(Messages.OperationFailedError);

            product.IsDeleted = true;
            product.LastUpdatedDate = DateTime.Now;

            _dbSet.Update(product);
            await _context.SaveChangesAsync();

            product.BannerFileName.RemoveFileFromServer(SiteTools.ProductBanner);

            foreach (var item in product.Gallery ?? new List<ProductGallery>())
            {
                item.FileName.RemoveFileFromServer(SiteTools.ProductGallery);
            }

            ProductDeletedEvent ProductDeletedEvent = new ProductDeletedEvent(
                product.Id);

            await _mediator.PublishEvent(ProductDeletedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterProductsViewModel>> AdminSideFilterProducts(AdminSideFilterProductsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterProductsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Product>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search) ||
                n.Slug.Contains(filter.Search) ||
                n.Description.Contains(filter.Search) ||
                n.Category.Title.Contains(filter.Search) ||
                n.ShortDescription.Contains(filter.Search));
            }

            if (filter.IsPublished is not null)
            {
                conditions.Add(n => n.IsPublished == filter.IsPublished);
            }

            if (filter.IsAvailable is not null)
            {
                conditions.Add(n => n.IsAvailable == filter.IsAvailable);
            }

            IQueryable<Product> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Product>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideProductViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                Slug = n.Slug,
                DisplayOrderPrice = n.DisplayOrderPrice,
                DisplayPrice = n.DisplayPrice,
                IsAvailable = n.IsAvailable,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
                Category = n.Category.Title,
                ViewsCount = n.Views.Count()
            }));

            return filter;
        }

        public async Task<Result<AdminSideUpdateProductViewModel>> AdminSideGetProductForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateProductViewModel>(Messages.OperationFailedError);

            var product = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUpdateProductViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                Slug = n.Slug,
                BannerFileName = n.BannerFileName,
                CategoryId = n.CategoryId,
                Description = n.Description,
                DisplayOrderPrice = n.DisplayOrderPrice,
                DisplayPrice = n.DisplayPrice,
                IsPublished = n.IsPublished,
                IsAvailable = n.IsAvailable,
                Priority = n.Priority,
                ShortDescription = n.ShortDescription,
            }).AsNoTracking().FirstOrDefaultAsync();
            if (product is null) return Result.Failure<AdminSideUpdateProductViewModel>(Messages.OperationFailedError);

            return product;
        }

        public async Task<Result> AdminSideUpdateProduct(AdminSideUpdateProductViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);
            if (await _dbSet.AnyAsync(n => n.Id != model.Id && n.Slug == model.Slug.Replace(" ", "-")))
                return Result.Failure(Messages.DuplicateSlugError);

            var product = await _dbSet.FindAsync(model.Id);
            if (product is null) return Result.Failure(Messages.OperationFailedError);

            if (model.BannerFile is not null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.BannerFile.FileName);
                if (!model.BannerFile.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
                if (model.BannerFile.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

                model.BannerFile.UplaodFileToServer(fileName, SiteTools.ProductBanner);
                product.BannerFileName.RemoveFileFromServer(SiteTools.ProductBanner);

                product.BannerFileName = fileName;
            }

            product.Title = model.Title;
            product.Slug = model.Slug.Replace(" ", "-");
            product.Description = model.Description;
            product.Priority = model.Priority;
            product.IsPublished = model.IsPublished;
            product.DisplayOrderPrice = model.DisplayOrderPrice;
            product.DisplayPrice = model.DisplayPrice;
            product.IsAvailable = model.IsAvailable;
            product.CategoryId = model.CategoryId;
            product.ShortDescription = model.ShortDescription;

            _dbSet.Update(product);
            await _context.SaveChangesAsync();

            ProductUpdatedEvent ProductUpdatedEvent = new ProductUpdatedEvent(
                product.Id,
                product.BannerFileName,
                product.Title,
                product.Slug,
                product.ShortDescription,
                product.Description,
                product.Priority,
                product.IsPublished,
                product.DisplayPrice,
                product.DisplayOrderPrice,
                product.IsAvailable);

            await _mediator.PublishEvent(ProductUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result<List<ClientSideProductViewModel>>> ClientSideGetLatestProducts()
        {
            var products = await _dbSet.Where(n => n.IsPublished).OrderByDescending(n => n.Id)
                .Take(16)
                .Select(n => new ClientSideProductViewModel()
                {
                    Id = n.Id,
                    BannerFileName = n.BannerFileName,
                    Title = n.Title,
                    Slug = n.Slug,
                    ShortDescription = n.ShortDescription,
                    Price = n.DisplayPrice,
                    HaveDiscount = ((n.DisplayOrderPrice != null) ? true : false),
                    OrderPrice = n.DisplayOrderPrice,
                    Percent = ((n.DisplayOrderPrice != null) ? n.DisplayPrice.GetDiscountPercent(n.DisplayOrderPrice.Value) : 0),
                }).AsNoTracking().ToListAsync();
            if (!products.Any()) return Result.Failure<List<ClientSideProductViewModel>>(Messages.OperationFailedError);

            var productIds = products.Select(n => n.Id).ToList();

            var reviewsResult = await _reviewService.GetProductsReviewRate(productIds);

            var wishListResult = await _wishListService.GetCurrentUserWishListIds();

            foreach (var item in products ?? new List<ClientSideProductViewModel>())
            {
                if (wishListResult.IsSuccess)
                {
                    var productId = wishListResult.Value.FirstOrDefault(n => n == item.Id);
                    if (productId > 0) item.IsInUserWishList = true;
                }

                if (reviewsResult.IsSuccess)
                {
                    var review = reviewsResult.Value.FirstOrDefault(n => n.ProductId == item.Id);
                    if (review is not null) item.ReviewRate = review.RoundedAvrage;
                }
            }

            return products;
        }

        public async Task<Result<ClientSideFilterProductsViewModel>> ClientSideFilterProducts(ClientSideFilterProductsViewModel filter)
        {
            if (filter is null) return Result.Failure<ClientSideFilterProductsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Product>();

            conditions.Add(n => n.IsPublished);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search) ||
                n.Slug.Contains(filter.Search) ||
                n.ShortDescription.Contains(filter.Search) ||
                n.Category.Title.Contains(filter.Search));
            }

            if (filter.CategoryId is not null)
            {
                conditions.Add(n => n.CategoryId == filter.CategoryId ||
                n.Category.ParentCategoryId == filter.CategoryId ||
                n.Category.ParentCategory.ParentCategoryId == filter.CategoryId);
            }

            IQueryable<Product> query = _dbSet;
            if (filter.Sort is not null)
            {
                switch (filter.Sort)
                {
                    case ProductSortTypeEnum.NewArrivals:
                        query = query.OrderByDescending(n => n.Id);
                        break;
                    case ProductSortTypeEnum.Popularity:
                        query = query.OrderBy(n => n.Views.Count());
                        break;
                    case ProductSortTypeEnum.PriceLowToHigh:
                        query = query.OrderBy(n => n.DisplayPrice);
                        break;
                    case ProductSortTypeEnum.PriceHighToLow:
                        query = query.OrderByDescending(n => n.DisplayPrice);
                        break;
                    default:
                        query = query.OrderByDescending(n => n.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(n => n.Id);
            }

            foreach (var item in conditions ?? new FilterConditions<Product>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideProductViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                Slug = n.Slug,
                BannerFileName = n.BannerFileName,
                ShortDescription = n.ShortDescription,
                HaveDiscount = ((n.DisplayOrderPrice != null) ? true : false),
                Price = n.DisplayPrice,
                OrderPrice = n.DisplayOrderPrice,
                Percent = ((n.DisplayOrderPrice != null) ? n.DisplayPrice.GetDiscountPercent(n.DisplayOrderPrice.Value) : 0)
            }));

            var productIds = filter.Entities.Select(n => n.Id).ToList();

            var wishListResult = await _wishListService.GetCurrentUserWishListIds();

            var reviewsResult = await _reviewService.GetProductsReviewRate(productIds);

            foreach (var item in filter.Entities ?? new List<ClientSideProductViewModel>())
            {
                if (wishListResult.IsSuccess)
                {
                    var id = wishListResult.Value.FirstOrDefault(n => n == item.Id);
                    if (id > 0) item.IsInUserWishList = true;
                }

                if (reviewsResult.IsSuccess)
                {
                    var review = reviewsResult.Value.FirstOrDefault(n => n.ProductId == item.Id);
                    if (review is not null) item.ReviewRate = review.RoundedAvrage;
                }
            }
            return filter;
        }

        public async Task<Result<List<ClientSideProductViewModel>>> ClientSideGetTopRatedProducts()
        {
            var products = await _dbSet.Where(n => n.IsPublished).OrderBy(n => n.Views.Count())
                .Take(5)
                .Select(n => new ClientSideProductViewModel()
                {
                    Id = n.Id,
                    BannerFileName = n.BannerFileName,
                    Title = n.Title,
                    Slug = n.Slug,
                    ShortDescription = n.ShortDescription,
                    HaveDiscount = ((n.DisplayOrderPrice != null) ? true : false),
                    Price = n.DisplayPrice,
                    OrderPrice = n.DisplayOrderPrice,
                    Percent = ((n.DisplayOrderPrice != null) ? n.DisplayPrice.GetDiscountPercent(n.DisplayOrderPrice.Value) : 0)
                }).AsNoTracking().ToListAsync();
            if (!products.Any()) return Result.Failure<List<ClientSideProductViewModel>>(Messages.OperationFailedError);

            var productIds = products.Select(n => n.Id).ToList();

            var reviewsResult = await _reviewService.GetProductsReviewRate(productIds);

            foreach (var item in products ?? new List<ClientSideProductViewModel>())
            {
                if (reviewsResult.IsSuccess)
                {
                    var review = reviewsResult.Value.FirstOrDefault(n => n.ProductId == item.Id);
                    if (review is not null) item.ReviewRate = review.RoundedAvrage;
                }
            }

            return products;
        }

        public async Task<Result<ClientSideProductDetailsViewModel>> ClientSideGetProductDetails(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return Result.Failure<ClientSideProductDetailsViewModel>(Messages.OperationFailedError);

            var product = await _dbSet.Where(n => n.Slug == slug && n.IsPublished)
                .Select(n => new ClientSideProductDetailsViewModel()
                {
                    Id = n.Id,
                    Title = n.Title,
                    Slug = n.Slug,
                    IsAvailable = n.IsAvailable,
                    ShortDescription = n.ShortDescription,
                    Description = n.Description,
                    BannerFileName = n.BannerFileName,
                    Gallery = n.Gallery.Select(n => n.FileName).ToList(),
                    HaveDiscount = ((n.DisplayOrderPrice != null) ? true : false),
                    OrderPrice = n.DisplayOrderPrice,
                    Price = n.DisplayPrice,
                    Category = n.Category.Title,
                    CategoryId = n.CategoryId,
                    Percent = ((n.DisplayOrderPrice != null) ? n.DisplayPrice.GetDiscountPercent(n.DisplayOrderPrice.Value) : 0),
                    Options = n.Options.Select(o => new ClientSideProductOptionViewModel()
                    {
                        Id = o.Id,
                        Title = o.Title,
                        Price = o.Price,
                        OrderPrice = o.OrderPrice
                    }).ToList(),
                    ReviewsCount = n.Reviews.Where(n => n.IsConfirmed).Count(),
                }).AsNoTracking().FirstOrDefaultAsync();
            if (product is null) return Result.Failure<ClientSideProductDetailsViewModel>(Messages.OperationFailedError);

            product.IsInUserWishList = await _wishListService.IsInUserWishList(product.Id);
            product.IsInUserCompare = await _compareService.IsInUserCompare(product.Id);

            return product;
        }

        public async Task<Result<List<ClientSideProductViewModel>>> ClientSideGetRelatedProducts(int id, int categoryId)
        {
            if (id < 1 || categoryId < 1) return Result.Failure<List<ClientSideProductViewModel>>(Messages.OperationFailedError);

            var products = await _dbSet.Where(n => n.IsPublished && n.Id != id && n.CategoryId == categoryId)
                .Take(8)
                .Select(n => new ClientSideProductViewModel()
                {
                    Id = n.Id,
                    Title = n.Title,
                    Slug = n.Slug,
                    BannerFileName = n.BannerFileName,
                    Price = n.DisplayPrice,
                    ShortDescription = n.ShortDescription,
                    HaveDiscount = ((n.DisplayOrderPrice != null) ? true : false),
                    OrderPrice = n.DisplayOrderPrice,
                    Percent = ((n.DisplayOrderPrice != null) ? n.DisplayPrice.GetDiscountPercent(n.DisplayOrderPrice.Value) : 0)
                }).AsNoTracking().ToListAsync();
            if (!products.Any()) return Result.Failure<List<ClientSideProductViewModel>>(Messages.OperationFailedError);

            var productIds = products.Select(n => n.Id).ToList();

            var reviewsResult = await _reviewService.GetProductsReviewRate(productIds);

            var wishListResult = await _wishListService.GetCurrentUserWishListIds();

            foreach (var item in products ?? new List<ClientSideProductViewModel>())
            {
                if (wishListResult.IsSuccess)
                {
                    var productId = wishListResult.Value.FirstOrDefault(n => n == item.Id);
                    if (productId > 0) item.IsInUserWishList = true;
                }

                if (reviewsResult.IsSuccess)
                {
                    var review = reviewsResult.Value.FirstOrDefault(n => n.ProductId == item.Id);
                    if (review is not null) item.ReviewRate = review.RoundedAvrage;
                }
            }

            return products;
        }
    }
}
