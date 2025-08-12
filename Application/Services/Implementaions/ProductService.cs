using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.Product.AdminSide;
using Data.Context;
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
        #endregion

        #region Ctor
        public ProductService(TrendicatorDbContext context,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<Product>();
            _mediator = mediator;
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
            };

            await _dbSet.AddAsync(product);
            await _context.SaveChangesAsync();

            ProductCreatedEvent ProductCreatedEvent = new ProductCreatedEvent(
                product.BannerFileName,
                product.Title,
                product.Slug,
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
                n.Category.Title.Contains(filter.Search));
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
                Priority = n.Priority
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

            _dbSet.Update(product);
            await _context.SaveChangesAsync();

            ProductUpdatedEvent ProductUpdatedEvent = new ProductUpdatedEvent(
                product.Id,
                product.BannerFileName,
                product.Title,
                product.Slug,
                product.Description,
                product.Priority,
                product.IsPublished,
                product.DisplayPrice,
                product.DisplayOrderPrice,
                product.IsAvailable);

            await _mediator.PublishEvent(ProductUpdatedEvent);

            return Result.Success();
        }
    }
}
