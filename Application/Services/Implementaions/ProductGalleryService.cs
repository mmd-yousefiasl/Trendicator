using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.ProductGallery.AdminSide;
using Data.Context;
using Domain.Models.Products;
using Domain.Shared;
using Domain.Statics;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class ProductGalleryService : IProductGalleryService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<ProductGallery> _dbSet;
        #endregion

        #region Ctor
        public ProductGalleryService(TrendicatorDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<ProductGallery>();
        }
        #endregion

        public async Task<Result<List<AdminSideProductGalleryViewModel>>> AdminSideGetProductGalleries(int id)
        {
            if (id < 1) return Result.Failure<List<AdminSideProductGalleryViewModel>>(Messages.OperationFailedError);
            var galleries = await _dbSet.Where(n => n.ProductId == id).Select(n => new AdminSideProductGalleryViewModel()
            {
                Id = n.Id,
                FileName = n.FileName,
                ProductId = n.ProductId
            }).AsNoTracking().OrderByDescending(n => n.Id).ToListAsync();
            if (!galleries?.Any() ?? true) return Result.Failure<List<AdminSideProductGalleryViewModel>>(Messages.OperationFailedError);

            return galleries;
        }

        public async Task<Result> CreateNewProductGallery(AdminSideCreateNewProductGalleryViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);
            if (!model.File.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
            if (model.File.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
            model.File.UplaodFileToServer(fileName, SiteTools.ProductGallery);

            ProductGallery gallery = new ProductGallery()
            {
                ProductId = model.ProductId,
                FileName = fileName
            };

            await _dbSet.AddAsync(gallery);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteProductGallery(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var gallery = await _dbSet.FindAsync(id);
            if (gallery is null) return Result.Failure(Messages.OperationFailedError);

            gallery.FileName.RemoveFileFromServer(SiteTools.ProductGallery);

            _dbSet.Remove(gallery);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
