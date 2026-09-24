using Application.Dtos.Media;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Models.SiteInfo;
using Domain.Shared;
using Domain.Statics;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class MediaService : IMediaService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Media> _dbSet;
        #endregion

        #region Ctor
        public MediaService(TrendicatorDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Media>();
        }
        #endregion

        public async Task<Result> AdminSideCreateNewMedia(AdminSideCreateNewMediaViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            if (!model.File.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
            if (model.File.Length > 5.CalculateMB()) return Result.Failure(Messages.FileLengthError);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
            model.File.UploadFileToServer(fileName, SiteTools.Media);

            var media = new Media()
            {
                FileName = fileName,
            };

            await _dbSet.AddAsync(media);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteMedia(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var media = await _dbSet.FindAsync(id);
            if (media is null) return Result.Failure(Messages.OperationFailedError);

            media.FileName.RemoveFileFromServer(SiteTools.Media);

            _dbSet.Remove(media);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterMediaViewModel>> AdminSideFilterMedia(AdminSideFilterMediaViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterMediaViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Media>();

            IQueryable<Media> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            await filter.Paging(query.Select(n => new AdminSideMediaViewModel()
            {
                Id = n.Id,
                FileName = n.FileName
            }));

            return filter;
        }
    }
}
