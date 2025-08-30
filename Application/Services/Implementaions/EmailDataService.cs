using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
using Application.ViewModels.EmailData.AdminSide;
using Data.Context;
using Domain.Events.SiteInfo;
using Domain.Models.SiteInfo;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class EmailDataService : IEmailDataService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<EmailData> _dbSet;
        private readonly IMediatorHandler _mediator;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor
        public EmailDataService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            ICacheManager cacheManager)
        {
            _context = context;
            _dbSet = _context.Set<EmailData>();
            _mediator = mediator;
            _cacheManager = cacheManager;
        }
        #endregion

        public async Task<Result<AdminSideEmailDataViewModel>> GetEmailData()
        {
            var data = await _cacheManager.GetAsync(CacheKeys.EmailDataCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.Id == 1).Select(n => new AdminSideEmailDataViewModel()
                {
                    Id = n.Id,
                    Smtp = n.Smtp,
                    EmailDisplayName = n.EmailDisplayName,
                    From = n.From,
                    Password = n.Password,
                    Port = n.Port ?? 1
                }).AsNoTracking().FirstOrDefaultAsync();
            });
            if (data is null) return Result.Failure<AdminSideEmailDataViewModel>(Messages.OperationFailedError);

            return data;
        }

        public async Task<Result<AdminSideUpdateEmailDataViewModel>> GetEmailDataForUpdate()
        {
            var emailData = await _dbSet.Where(n => n.Id == 1).Select(n => new AdminSideUpdateEmailDataViewModel()
            {
                Id = n.Id,
                Smtp = n.Smtp,
                EmailDisplayName = n.EmailDisplayName,
                From = n.From,
                Password = n.Password,
                Port = n.Port ?? 1,
            }).AsNoTracking().FirstOrDefaultAsync();
            if (emailData is null) return Result.Failure<AdminSideUpdateEmailDataViewModel>(Messages.OperationFailedError);

            return emailData;
        }

        public async Task<Result> UpdateEmailData(AdminSideUpdateEmailDataViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var emailData = await _dbSet.FindAsync(1);
            if (emailData is null) return Result.Failure(Messages.OperationFailedError);

            emailData.From = model.From;
            emailData.Password = model.Password;
            emailData.EmailDisplayName = model.EmailDisplayName;
            emailData.Smtp = model.Smtp;
            emailData.Port = model.Port;

            _dbSet.Update(emailData);
            await _context.SaveChangesAsync();

            EmailDataUpdatedEvent EmailDataUpdatedEvent = new EmailDataUpdatedEvent(
                emailData.Id,
                emailData.From,
                emailData.Password,
                emailData.EmailDisplayName,
                emailData.Smtp,
                emailData.Port);

            await _mediator.PublishEvent(EmailDataUpdatedEvent);

            return Result.Success();
        }
    }
}
