using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
using Application.ViewModels.SmsData.AdminSide;
using Data.Context;
using Domain.Events.SiteInfo;
using Domain.Models.SiteInfo;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class SmsDataService : ISmsDataService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<SmsData> _dbSet;
        private readonly IMediatorHandler _mediator;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor
        public SmsDataService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            ICacheManager cacheManager)
        {
            _context = context;
            _dbSet = _context.Set<SmsData>();
            _mediator = mediator;
            _cacheManager = cacheManager;
        }
        #endregion

        public async Task<Result<AdminSideSmsDataViewModel>> AdminSideGetSmsData()
        {
            var data = await _cacheManager.GetAsync(CacheKeys.SmsDataCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.Id == 1)
                .Select(n => new AdminSideSmsDataViewModel()
                {
                    SmsPanelAPIKey = n.SmsPanelAPIKey,
                    SmsPatternIdForLoginCode = n.SmsPatternIdForLoginCode
                }).AsNoTracking().FirstOrDefaultAsync();
            });
            if (data is null ||
                string.IsNullOrWhiteSpace(data.SmsPanelAPIKey) ||
                data.SmsPatternIdForLoginCode is null)
                return Result.Failure<AdminSideSmsDataViewModel>(Messages.OperationFailedError);

            return data;
        }

        public async Task<Result<AdminSideUpdateSmsDataViewModel>> AdminSideGetSmsDataForUpdate()
        {
            var data = await _dbSet.Where(n => n.Id == 1)
                .Select(n => new AdminSideUpdateSmsDataViewModel()
                {
                    Id = n.Id,
                    SmsPanelAPIKey = n.SmsPanelAPIKey,
                    SmsPatternIdForLoginCode = n.SmsPatternIdForLoginCode ?? 0
                }).AsNoTracking().FirstOrDefaultAsync();
            if (data is null) return Result.Failure<AdminSideUpdateSmsDataViewModel>(Messages.OperationFailedError);

            return data;
        }

        public async Task<Result> AdminSideUpdateSmsData(AdminSideUpdateSmsDataViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var data = await _dbSet.FindAsync(1);
            if (data is null) return Result.Failure(Messages.OperationFailedError);

            data.SmsPanelAPIKey = model.SmsPanelAPIKey;
            data.SmsPatternIdForLoginCode = model.SmsPatternIdForLoginCode;

            _dbSet.Update(data);
            await _context.SaveChangesAsync();

            SmsDataUpdatedEvent SmsDataUpdatedEvent = new SmsDataUpdatedEvent(
                data.SmsPanelAPIKey,
                data.SmsPatternIdForLoginCode);

            await _mediator.PublishEvent(SmsDataUpdatedEvent);

            return Result.Success();
        }
    }
}
