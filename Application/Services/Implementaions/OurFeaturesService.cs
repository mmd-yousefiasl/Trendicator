using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.OurFeature.AdminSide;
using Application.ViewModels.OurFeature.ClientSide;
using Data.Context;
using Domain.Events.SiteInfo.OurFeature;
using Domain.Models.SiteInfo;
using Domain.Shared;
using Domain.Statics;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class OurFeaturesService : IOurFeaturesService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<OurFeature> _dbSet;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public OurFeaturesService(TrendicatorDbContext context,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<OurFeature>();
            _mediator = mediator;
        }
        #endregion

        public async Task<Result> CreateNewOurFeature(AdminSideCreateNewOurFeatureViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            if (!model.File.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
            if (model.File.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
            model.File.UplaodFileToServer(fileName, SiteTools.OurFeature);

            var ourFeature = new OurFeature()
            {
                Title = model.Title,
                Description = model.Description,
                IsPublished = model.IsPublished,
                Priority = model.Priority,
                FileName = fileName
            };

            await _dbSet.AddAsync(ourFeature);
            await _context.SaveChangesAsync();

            OurFeatureCreatedEvent OurFeatureCreatedEvent = new OurFeatureCreatedEvent(
                ourFeature.Title,
                ourFeature.Description,
                ourFeature.FileName,
                ourFeature.Priority,
                ourFeature.IsPublished);

            await _mediator.PublishEvent(OurFeatureCreatedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterOurFeaturesViewModel>> FilterOurFeatures(AdminSideFilterOurFeaturesViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterOurFeaturesViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<OurFeature>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search) || n.Description.Contains(filter.Search));
            }

            if (filter.IsPublished is not null)
            {
                conditions.Add(n => n.IsPublished == filter.IsPublished);
            }

            IQueryable<OurFeature> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<OurFeature>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideOurFeatureViewModel()
            {
                Id = n.Id,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
                Title = n.Title
            }));

            return filter;
        }

        public async Task<Result<AdminSideUpdateOurFeatureViewModel>> GetOurFeatureForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateOurFeatureViewModel>(Messages.OperationFailedError);

            var ourFeature = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUpdateOurFeatureViewModel()
            {
                Id = n.Id,
                FileName = n.FileName,
                Description = n.Description,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
                Title = n.Title
            }).FirstOrDefaultAsync();
            if (ourFeature is null) return Result.Failure<AdminSideUpdateOurFeatureViewModel>(Messages.OperationFailedError);

            return ourFeature;
        }

        public async Task<Result<List<ClientSideOurFeatureViewModel>>> GetOurFeatures()
        {
            var features = await _dbSet.Where(n => n.IsPublished).OrderBy(n => n.Priority)
                .Select(n => new ClientSideOurFeatureViewModel()
                {
                    Title = n.Title,
                    FileName = n.FileName,
                    Description = n.Description,
                }).AsNoTracking().Take(4).ToListAsync();
            if (!features?.Any() ?? true) return Result.Failure<List<ClientSideOurFeatureViewModel>>(Messages.OperationFailedError);

            return features;
        }

        public async Task<Result> SoftDeleteOurFeature(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var feature = await _dbSet.FindAsync(id);
            if (feature is null) return Result.Failure(Messages.OperationFailedError);

            feature.FileName.RemoveFileFromServer(SiteTools.OurFeature);

            _dbSet.Remove(feature);
            await _context.SaveChangesAsync();

            OurFeatureDeletedEvent OurFeatureDeletedEvent = new OurFeatureDeletedEvent(
                feature.Id);

            await _mediator.PublishEvent(OurFeatureDeletedEvent);

            return Result.Success();
        }

        public async Task<Result> UpdateOurFeature(AdminSideUpdateOurFeatureViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var feature = await _dbSet.FindAsync(model.Id);
            if (feature is null) return Result.Failure(Messages.OperationFailedError);

            if (model.File is not null)
            {
                if (!model.File.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
                if (model.File.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.OperationFailedError);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
                model.File.UplaodFileToServer(fileName, SiteTools.OurFeature);
                feature.FileName.RemoveFileFromServer(SiteTools.OurFeature);

                feature.FileName = fileName;
            }

            feature.Title = model.Title;
            feature.Description = model.Description;
            feature.Priority = model.Priority;
            feature.IsPublished = model.IsPublished;

            _dbSet.Update(feature);
            await _context.SaveChangesAsync();

            OurFeatureUpdatedEvent OurFeatureUpdatedEvent = new OurFeatureUpdatedEvent(
            feature.Id,
            feature.Title,
            feature.Description,
            feature.FileName,
            feature.Priority,
            feature.IsPublished);

            await _mediator.PublishEvent(OurFeatureUpdatedEvent);

            return Result.Success();
        }
    }
}
