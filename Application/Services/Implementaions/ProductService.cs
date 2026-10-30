using Application.Dtos.Product.AdminSide;
using Application.Dtos.Product.ClientSide;
using Application.Dtos.ProductDetails.AdminSide;
using Application.Dtos.ProductDetails.ClientSide;
using Application.Dtos.ProductFaq.ClientSide;
using Application.Dtos.ProductFeature.AdminSide;
using Application.Dtos.ProductReview.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Enums.Products;
using Domain.Events.Products.Product;
using Domain.Models.Products;
using Domain.Shared;
using Domain.Statics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class ProductService : IProductService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Product> _dbSet;
        private readonly IMediatorHandler _mediator;
        private readonly IProductReviewService _reviewService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IVisitorService _visitorService;
        #endregion

        #region Ctor
        public ProductService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            IProductReviewService reviewService,
            IHttpContextAccessor httpContextAccessor,
            IVisitorService visitorService)
        {
            _context = context;
            _dbSet = _context.Set<Product>();
            _mediator = mediator;
            _reviewService = reviewService;
            _httpContextAccessor = httpContextAccessor;
            _visitorService = visitorService;
        }
        #endregion

        #region Product
        public async Task<Result> AdminSideCreateNewProduct(AdminSideCreateNewProductViewModel model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            model.Slug = model.Slug.Replace(" ", "-");

            if (await _dbSet.AnyAsync(n => n.Slug == model.Slug))
                return Result.Failure(Messages.DuplicateSlugError);

            if (!model.BannerFile.IsImage())
                return Result.Failure(Messages.InvalidFileFormatError);

            if (model.BannerFile.Length > SiteTools.MaxImageFileLength.CalculateMB())
                return Result.Failure(Messages.FileLengthError);

            var product = new Product
            {
                Title = model.Title,
                Slug = model.Slug,
                Description = model.Description,
                DisplayPrice = model.DisplayPrice,
                OrderPrice = model.OrderPrice,
                IsAvailable = model.IsAvailable,
                IsPublished = model.IsPublished,
                Priority = model.Priority,
                CategoryId = model.CategoryId,
                ShortDescription = model.ShortDescription,
                Type = model.Type,
                Hint = model.Hint,
            };
            if (model.Type == ProductTypeEnum.MetaTraderLink) product.MetaTraderLink = model.MetaTraderLink;

            if (model.IndicatorFile is not null && model.Type == ProductTypeEnum.IndicatorFile)
            {
                if (!model.IndicatorFile.IsCompressedFile())
                    return Result.Failure(Messages.InvalidFileFormatError);

                if (model.IndicatorFile.Length > SiteTools.MaxImageFileLength.CalculateMB())
                    return Result.Failure(Messages.FileLengthError);

                var indicatorFileName = Guid.NewGuid() + Path.GetExtension(model.IndicatorFile.FileName);
                model.IndicatorFile.UploadSensitiveFileToServer(indicatorFileName, SiteTools.ProductIndicator);
                product.IndicatorFileName = indicatorFileName;
            }

            var bannerFileName = Guid.NewGuid() + Path.GetExtension(model.BannerFile.FileName);
            model.BannerFile.UploadFileToServer(bannerFileName, SiteTools.ProductBanner);
            product.BannerFileName = bannerFileName;

            await _dbSet.AddAsync(product);
            await _context.SaveChangesAsync();

            await _mediator.PublishEvent(new ProductCreatedEvent(
                product.BannerFileName,
                product.Title,
                product.Slug,
                product.ShortDescription,
                product.Description,
                product.Priority,
                product.IsPublished,
                product.DisplayPrice,
                product.OrderPrice,
                product.IsAvailable,
                product.Type,
                product.MetaTraderLink,
                product.IndicatorFileName
            ));

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteProduct(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var product = await _dbSet.Where(n => n.Id == id).FirstOrDefaultAsync();
            if (product is null) return Result.Failure(Messages.OperationFailedError);

            product.IsDeleted = true;
            product.LastUpdatedDate = DateTime.UtcNow;

            _dbSet.Update(product);
            await _context.SaveChangesAsync();

            product.BannerFileName.RemoveFileFromServer(SiteTools.ProductBanner);
            product.IndicatorFileName?.RemoveSensitiveFileFromServer(SiteTools.ProductIndicator);

            ProductDeletedEvent ProductDeletedEvent = new ProductDeletedEvent(
                product.Id);

            await _mediator.PublishEvent(ProductDeletedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterProductsViewModel>> AdminSideFilterProducts(AdminSideFilterProductsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterProductsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Product>();

            if (filter.Type is not null)
            {
                conditions.Add(n => n.Type == filter.Type);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search));
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
                OrderPrice = n.OrderPrice,
                DisplayPrice = n.DisplayPrice,
                IsAvailable = n.IsAvailable,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
                Category = n.Category.Title,
                ViewsCount = n.ViewsCount,
                Type = n.Type
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
                OrderPrice = n.OrderPrice,
                DisplayPrice = n.DisplayPrice,
                IsPublished = n.IsPublished,
                IsAvailable = n.IsAvailable,
                Priority = n.Priority,
                ShortDescription = n.ShortDescription,
                Type = n.Type,
                MetaTraderLink = n.MetaTraderLink,
                IndicatorFileName = n.IndicatorFileName,
                Hint = n.Hint,
            }).AsNoTracking().FirstOrDefaultAsync();
            if (product is null) return Result.Failure<AdminSideUpdateProductViewModel>(Messages.OperationFailedError);

            return product;
        }

        public async Task<Result> AdminSideUpdateProduct(AdminSideUpdateProductViewModel model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            model.Slug = model.Slug.Replace(" ", "-");

            bool isDuplicateSlug = await _dbSet.AnyAsync(n => n.Id != model.Id && n.Slug == model.Slug);
            if (isDuplicateSlug)
                return Result.Failure(Messages.DuplicateSlugError);

            var product = await _dbSet.FindAsync(model.Id);
            if (product is null)
                return Result.Failure(Messages.OperationFailedError);

            // Handle Banner File
            if (model.BannerFile is not null)
            {
                if (!model.BannerFile.IsImage())
                    return Result.Failure(Messages.InvalidFileFormatError);

                if (model.BannerFile.Length > SiteTools.MaxImageFileLength.CalculateMB())
                    return Result.Failure(Messages.FileLengthError);

                var bannerFileName = Guid.NewGuid() + Path.GetExtension(model.BannerFile.FileName);
                model.BannerFile.UploadFileToServer(bannerFileName, SiteTools.ProductBanner);
                product.BannerFileName.RemoveFileFromServer(SiteTools.ProductBanner);
                product.BannerFileName = bannerFileName;
            }

            // Handle Indicator File
            if (model.IndicatorFile is not null && model.Type == ProductTypeEnum.IndicatorFile)
            {
                if (!model.IndicatorFile.IsCompressedFile())
                    return Result.Failure(Messages.InvalidFileFormatError);

                if (model.IndicatorFile.Length > SiteTools.MaxImageFileLength.CalculateMB())
                    return Result.Failure(Messages.FileLengthError);

                var indicatorFileName = Guid.NewGuid() + Path.GetExtension(model.IndicatorFile.FileName);
                model.IndicatorFile.UploadSensitiveFileToServer(indicatorFileName, SiteTools.ProductIndicator);
                product.IndicatorFileName?.RemoveSensitiveFileFromServer(SiteTools.ProductIndicator);
                product.IndicatorFileName = indicatorFileName;
            }

            // Update product fields
            product.Title = model.Title;
            product.Slug = model.Slug;
            product.Description = model.Description;
            product.Priority = model.Priority;
            product.IsPublished = model.IsPublished;
            product.OrderPrice = model.OrderPrice;
            product.DisplayPrice = model.DisplayPrice;
            product.IsAvailable = model.IsAvailable;
            product.CategoryId = model.CategoryId;
            product.ShortDescription = model.ShortDescription;
            product.Type = model.Type;
            product.Hint = model.Hint;

            if (model.Type == ProductTypeEnum.MetaTraderLink) product.MetaTraderLink = model.MetaTraderLink;

            _dbSet.Update(product);
            await _context.SaveChangesAsync();

            await _mediator.PublishEvent(new ProductUpdatedEvent(
                product.Id,
                product.BannerFileName,
                product.Title,
                product.Slug,
                product.ShortDescription,
                product.Description,
                product.Priority,
                product.IsPublished,
                product.DisplayPrice,
                product.OrderPrice,
                product.IsAvailable,
                product.Type,
                product.MetaTraderLink,
                product.IndicatorFileName
            ));

            return Result.Success();
        }

        public async Task<Result<ClientSideFilterProductsViewModel>> ClientSideFilterProducts(ClientSideFilterProductsViewModel filter)
        {
            if (filter is null) return Result.Failure<ClientSideFilterProductsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Product>();

            conditions.Add(n => n.IsPublished);

            if (!string.IsNullOrWhiteSpace(filter.Category))
            {
                conditions.Add(n => n.Category.Slug == filter.Category ||
                n.Category.ParentCategory.Slug == filter.Category ||
                n.Category.ParentCategory.ParentCategory.Slug == filter.Category);
            }

            IQueryable<Product> query = _context.Products;
            if (filter.Sort is not null)
            {
                switch (filter.Sort)
                {
                    case ProductSortTypeEnum.NewArrivals:
                        query = query.OrderByDescending(n => n.Id);
                        break;
                    case ProductSortTypeEnum.Popularity:
                        query = query.OrderBy(n => n.ViewsCount);
                        break;
                    case ProductSortTypeEnum.PriceLowToHigh:
                        conditions.Add(n => n.IsAvailable);
                        query = query.OrderBy(n => n.OrderPrice);
                        break;
                    case ProductSortTypeEnum.PriceHighToLow:
                        conditions.Add(n => n.IsAvailable);
                        query = query.OrderByDescending(n => n.OrderPrice);
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
                IsAvailable = n.IsAvailable,
                Category = n.Category.Title,
                ParentCategory = n.Category.ParentCategory.Title,
                CategorySlug = n.Category.Slug,
                ParentCategorySlug = n.Category.ParentCategory.Slug,
                ReviewRate = n.ReviewRate,
                HaveDiscount = ((n.DisplayPrice > 0) ? true : false),
                DisplayPrice = n.DisplayPrice,
                OrderPrice = n.OrderPrice,
                Percent = ((n.DisplayPrice > 0) ? n.DisplayPrice.GetDiscountPercent(n.OrderPrice) : 0),
            }));

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
                    HaveDiscount = ((n.DisplayPrice > 0) ? true : false),
                    DisplayPrice = n.DisplayPrice,
                    OrderPrice = n.OrderPrice,
                    Percent = ((n.DisplayPrice > 0) ? n.DisplayPrice.GetDiscountPercent(n.OrderPrice) : 0),
                    IsAvailable = n.IsAvailable,
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

        public async Task<Result<ClientSideProductDetailsViewModel>> ClientSideGetProductDetails(int Id)
        {
            if (Id < 1)
                return Result.Failure<ClientSideProductDetailsViewModel>(Messages.OperationFailedError);

            var product = await _context.Products
                .AsNoTracking()
                .AsSplitQuery()
                .Where(n => n.Id == Id && n.IsPublished)
                .Select(n => new ClientSideProductDetailsViewModel()
                {
                    Id = n.Id,
                    Title = n.Title,
                    Slug = n.Slug,
                    IsAvailable = n.IsAvailable,
                    Hint = n.Hint,
                    ShortDescription = n.ShortDescription,
                    Description = n.Description,
                    BannerFileName = n.BannerFileName,
                    Category = n.Category.Title,
                    CategorySlug = n.Category.Slug,
                    CategoryId = n.CategoryId,
                    ParentCategory = n.Category.ParentCategory.Title,
                    Faqs = n.Faqs
                    .Where(f => f.IsPublished)
                    .OrderBy(f => f.Priority)
                    .Select(f => new ClientSideProductFaqViewModel()
                    {
                        Question = f.Question,
                        Answer = f.Answer
                    }).ToList(),
                    Features = n.Features
                    .Where(f => f.IsPublished)
                    .OrderBy(f => f.Priority)
                    .Select(f => f.Text)
                    .ToList(),
                    Details = n.Details
                    .Where(d => d.IsPublished)
                    .OrderBy(d => d.Priority)
                    .Select(d => new ClientSideProductDetailsDto()
                    {
                        Title = d.Title,
                        Description = d.Description
                    }).ToList(),
                    Reviews = n.Reviews
                    .Where(r => r.IsPublished)
                    .OrderByDescending(r => r.Id)
                    .Take(10)
                    .Select(r => new ClientSideProductReviewDto()
                    {
                        Name = r.Name,
                        Starts = r.Stars,
                        Review = r.Review,
                        CreatedDate = r.CreatedDate,
                    }).ToList(),
                    ReviewsCount = n.ReviewsCount,
                    ReviewRate = n.ReviewRate,
                    ViewsCount = n.ViewsCount,
                    PurchaseCount = n.PurchaseCount,
                    HaveDiscount = ((n.DisplayPrice > 0) ? true : false),
                    OrderPrice = n.OrderPrice,
                    DisplayPrice = n.DisplayPrice,
                    Percent = ((n.DisplayPrice > 0) ? n.DisplayPrice.GetDiscountPercent(n.OrderPrice) : 0),
                }).FirstOrDefaultAsync();

            if (product is null)
                return Result.Failure<ClientSideProductDetailsViewModel>(Messages.OperationFailedError);

            return product;
        }

        public async Task<Result<ClientSideProductDetailsViewModel>> ClientSideGetProductDetails(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return Result.Failure<ClientSideProductDetailsViewModel>(Messages.OperationFailedError);

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
                    HaveDiscount = ((n.DisplayPrice > 0) ? true : false),
                    OrderPrice = n.OrderPrice,
                    DisplayPrice = n.DisplayPrice,
                    Category = n.Category.Title,
                    CategorySlug = n.Category.Slug,
                    CategoryId = n.CategoryId,
                    Percent = ((n.DisplayPrice > 0) ? n.DisplayPrice.GetDiscountPercent(n.OrderPrice) : 0),
                    Faqs = n.Faqs.Where(f => f.IsPublished).OrderBy(f => f.Priority)
                    .Select(f => new ClientSideProductFaqViewModel()
                    {
                        Question = f.Question,
                        Answer = f.Answer
                    }).ToList(),
                    ReviewsCount = n.Reviews.Where(n => n.IsPublished).Count(),
                }).AsNoTracking().FirstOrDefaultAsync();
            if (product is null) return Result.Failure<ClientSideProductDetailsViewModel>(Messages.OperationFailedError);

            return product;
        }

        public async Task<Result<List<ClientSideProductViewModel>>> ClientSideGetRelatedProducts(int id, int categoryId)
        {
            if (id < 1 || categoryId < 1)
                return Result.Failure<List<ClientSideProductViewModel>>(Messages.OperationFailedError);

            var products = await _context.Products
                .AsNoTracking()
                .Where(n => n.IsPublished && n.Id != id && n.CategoryId == categoryId)
                .OrderBy(n => n.Priority)
                .Take(3)
                .Select(n => new ClientSideProductViewModel()
                {
                    Id = n.Id,
                    Title = n.Title,
                    Slug = n.Slug,
                    BannerFileName = n.BannerFileName,
                    DisplayPrice = n.DisplayPrice,
                    ShortDescription = n.ShortDescription,
                    Category = n.Category.Title,
                    CategorySlug = n.Category.Slug,
                    ParentCategory = n.Category.ParentCategory.Title,
                    ParentCategorySlug = n.Category.ParentCategory.Slug,
                    ReviewRate = n.ReviewRate,
                    HaveDiscount = ((n.DisplayPrice > 0) ? true : false),
                    OrderPrice = n.OrderPrice,
                    Percent = ((n.DisplayPrice > 0) ? n.DisplayPrice.GetDiscountPercent(n.OrderPrice) : 0),
                    IsAvailable = n.IsAvailable,
                }).ToListAsync();

            if (!products.Any())
                return Result.Failure<List<ClientSideProductViewModel>>(Messages.OperationFailedError);

            return products;
        }

        public async Task<Result<int>> ClientSideGetProductIdByOptionId(int optionId)
        {
            if (optionId < 1) return Result.Failure<int>(Messages.OperationFailedError);

            return Result.Failure<int>(Messages.OperationFailedError);
        }

        public async Task<Result<List<ClientSideProductViewModel>>> ClientSideGetDiscountedProducts()
        {
            var products = await _dbSet.Where(n => n.IsPublished && n.OrderPrice != null).OrderByDescending(n => n.Id)
                .Take(6)
                .Select(n => new ClientSideProductViewModel()
                {
                    Id = n.Id,
                    BannerFileName = n.BannerFileName,
                    Title = n.Title,
                    Slug = n.Slug,
                    ShortDescription = n.ShortDescription,
                    DisplayPrice = n.DisplayPrice,
                    HaveDiscount = ((n.DisplayPrice > 0) ? true : false),
                    OrderPrice = n.OrderPrice,
                    Percent = ((n.DisplayPrice > 0) ? n.DisplayPrice.GetDiscountPercent(n.OrderPrice) : 0),
                    IsAvailable = n.IsAvailable,
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

        public async Task<Result<List<ClientSideProductViewModel>>> ClientSideGetMostViewedProducts()
        {
            var products = await _dbSet.Where(n => n.IsPublished).OrderBy(n => n.Views.Count())
                .Take(6)
                .Select(n => new ClientSideProductViewModel()
                {
                    Id = n.Id,
                    BannerFileName = n.BannerFileName,
                    Title = n.Title,
                    Slug = n.Slug,
                    ShortDescription = n.ShortDescription,
                    DisplayPrice = n.DisplayPrice,
                    HaveDiscount = ((n.DisplayPrice > 0) ? true : false),
                    OrderPrice = n.OrderPrice,
                    Percent = ((n.DisplayPrice > 0) ? n.DisplayPrice.GetDiscountPercent(n.OrderPrice) : 0),
                    IsAvailable = n.IsAvailable,
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

        public async Task<Result<List<ClientSideProductViewModel>>> ClientSideGetMostPurchasedProducts()
        {
            var products = await _dbSet.Where(n => n.IsPublished).OrderBy(n => n.CartItems.Count())
                   .Take(6)
                   .Select(n => new ClientSideProductViewModel()
                   {
                       Id = n.Id,
                       BannerFileName = n.BannerFileName,
                       Title = n.Title,
                       Slug = n.Slug,
                       ShortDescription = n.ShortDescription,
                       DisplayPrice = n.DisplayPrice,
                       HaveDiscount = ((n.DisplayPrice > 0) ? true : false),
                       OrderPrice = n.OrderPrice,
                       Percent = ((n.DisplayPrice > 0) ? n.DisplayPrice.GetDiscountPercent(n.OrderPrice) : 0),
                       IsAvailable = n.IsAvailable,
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

        public async Task<Result<List<AdminSideProductForSiteMapViewModel>>> AdminSideGetProductIdsForSiteMap()
        {
            var ids = await _dbSet.Where(n => n.IsPublished)
                .Select(n => new AdminSideProductForSiteMapViewModel()
                {
                    Id = n.Id,
                    Slug = n.Slug
                })
                .AsNoTracking()
                .ToListAsync();
            if (!ids.Any())
                return Result.Failure<List<AdminSideProductForSiteMapViewModel>>(Messages.OperationFailedError);

            return ids;
        }

        public async Task<Result> ClientSideAddProductView(int productId)
        {
            var visitorId = _visitorService.GetOrCreateVisitorId();
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var alreadyViewed = await _context.ProductViews
                .AsNoTracking()
                .AnyAsync(n => n.ProductId == productId && n.VisitorId == visitorId);

            if (alreadyViewed)
                return Result.Success();

            var view = new ProductView()
            {
                ProductId = productId,
                UserId = userId > 0 ? userId : null,
                UserIP = userIP,
                VisitorId = visitorId
            };

            _context.ProductViews.Add(view);
            await _context.SaveChangesAsync();

            try
            {
                await _context.Products
                    .Where(b => b.Id == productId)
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.ViewsCount, b => b.ViewsCount + 1));
            }
            catch
            {
                return Result.Failure(Messages.OperationFailedError);
            }

            return Result.Success();
        }
        #endregion

        #region Product Feature

        public async Task<Result> AdminSideCreateNewProductFeature(AdminSideCreateNewProductFeatureDto model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            var feature = new ProductFeature()
            {
                Text = model.Text,
                Priority = model.Priority,
                IsPublished = model.IsPublished,
                ProductId = model.ProductId
            };

            _context.ProductFeatures.Add(feature);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteProductFeature(int id)
        {
            if (id < 1)
                return Result.Failure(Messages.OperationFailedError);

            var feature = await _context.ProductFeatures.FindAsync(id);
            if (feature is null)
                return Result.Failure(Messages.OperationFailedError);

            _context.ProductFeatures.Remove(feature);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<AdminSideUpdateProductFeatureDto>> AdminSideGetProductFeatureForUpdate(int id)
        {
            if (id < 1)
                return Result.Failure<AdminSideUpdateProductFeatureDto>(Messages.OperationFailedError);

            var feature = await _context.ProductFeatures
                .AsNoTracking()
                .Where(n => n.Id == id)
                .Select(n => new AdminSideUpdateProductFeatureDto()
                {
                    Id = n.Id,
                    IsPublished = n.IsPublished,
                    Priority = n.Priority,
                    ProductId = n.ProductId,
                    Text = n.Text,
                }).FirstOrDefaultAsync();

            if (feature is null)
                return Result.Failure<AdminSideUpdateProductFeatureDto>(Messages.OperationFailedError);

            return feature;
        }

        public async Task<Result> AdminSideUpdateProductFeature(AdminSideUpdateProductFeatureDto model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            var feature = await _context.ProductFeatures.FindAsync(model.Id);
            if (feature is null)
                return Result.Failure(Messages.OperationFailedError);

            feature.Text = model.Text;
            feature.Priority = model.Priority;
            feature.IsPublished = model.IsPublished;

            _context.ProductFeatures.Update(feature);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterProductFeaturesDto>> AdminSideFilterProductFeatures(AdminSideFilterProductFeaturesDto filter)
        {
            if (filter is null)
                return Result.Failure<AdminSideFilterProductFeaturesDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<ProductFeature>();

            conditions.Add(n => n.ProductId == filter.ProductId);

            IQueryable<ProductFeature> query = _context.ProductFeatures;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<ProductFeature>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideProductFeatureDto()
            {
                Id = n.Id,
                Text = n.Text,
                Priority = n.Priority,
                IsPublished = n.IsPublished,
                ProductId = n.ProductId
            }));

            return filter;
        }

        #endregion

        #region Product Details
        public async Task<Result> AdminSideCreateNewProductDeatils(AdminSideCreateNewProductDetailsDto model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            var details = new ProductDetails()
            {
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority,
                IsPublished = model.IsPublished,
                ProductId = model.ProductId,
            };

            _context.ProductDetails.Add(details);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteProductDetails(int id)
        {
            if (id < 1)
                return Result.Failure(Messages.OperationFailedError);

            var details = await _context.ProductDetails.FindAsync(id);
            if (details is null)
                return Result.Failure(Messages.OperationFailedError);

            _context.ProductDetails.Remove(details);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterProductDetailsDto>> AdminSideFilterProductDetails(AdminSideFilterProductDetailsDto filter)
        {
            if (filter is null)
                return Result.Failure<AdminSideFilterProductDetailsDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<ProductDetails>();

            conditions.Add(n => n.ProductId == filter.ProductId);

            IQueryable<ProductDetails> query = _context.ProductDetails;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<ProductDetails>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideProductDetailsDto()
            {
                Id = n.Id,
                Title = n.Title,
                Description = n.Description,
                Priority = n.Priority,
                IsPublished = n.IsPublished,
                ProductId = n.ProductId
            }));

            return filter;
        }

        public async Task<Result<AdminSideUpdateProductDetailsDto>> AdminSideGetProductDetailsForUpdate(int id)
        {
            if (id < 1)
                return Result.Failure<AdminSideUpdateProductDetailsDto>(Messages.OperationFailedError);

            var details = await _context.ProductDetails
                .AsNoTracking()
                .Where(n => n.Id == id)
                .Select(n => new AdminSideUpdateProductDetailsDto()
                {
                    Id = n.Id,
                    Title = n.Title,
                    Description = n.Description,
                    IsPublished = n.IsPublished,
                    Priority = n.Priority,
                    ProductId = n.ProductId
                }).FirstOrDefaultAsync();

            if (details is null)
                return Result.Failure<AdminSideUpdateProductDetailsDto>(Messages.OperationFailedError);

            return details;
        }

        public async Task<Result> AdminSideUpdateProductDetails(AdminSideUpdateProductDetailsDto model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            var details = await _context.ProductDetails.FindAsync(model.Id);
            if (details is null)
                return Result.Failure(Messages.OperationFailedError);

            details.Title = model.Title;
            details.Description = model.Description;
            details.IsPublished = model.IsPublished;
            details.Priority = model.Priority;

            _context.ProductDetails.Update(details);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        #endregion

        #region Product Comment
        public async Task<Result> ClientSideAddProductComment(ClientSideAddProductCommentDto model)
        {
            if (model is null || model.ProductId < 1)
                return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            var comment = new ProductReview()
            {
                ProductId = model.ProductId,
                Name = model.Name,
                Email = model.Email,
                Review = model.Review,
                IsPublished = false,
                Reply = null,
                UserIP = userIP,
                Stars = model.Stars,
            };

            if (userId > 0)
                comment.UserId = userId;


            _context.ProductReviews.Add(comment);
            await _context.SaveChangesAsync();

            return Result.Success(Messages.CommentSentSuccessfully);
        }
        #endregion
    }
}
