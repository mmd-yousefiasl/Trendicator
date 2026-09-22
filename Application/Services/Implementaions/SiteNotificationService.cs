using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.SiteNotifications.AdminSide;
using Application.ViewModels.SiteNotifications.ClientSide;
using Data.Context;
using Domain.Models.SiteNotifications;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class SiteNotificationService : ISiteNotificationService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITelegramService _telegramService;
        #endregion

        #region Ctor
        public SiteNotificationService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ITelegramService telegramService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _telegramService = telegramService;
        }
        #endregion

        public async Task<Result<AdminSideFilterNotificationsDto>> AdminSideFilterNotifications(AdminSideFilterNotificationsDto filter)
        {
            if (filter is null)
                return Result.Failure<AdminSideFilterNotificationsDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<SiteNotification>();

            if (filter.Type is not null)
            {
                conditions.Add(n => n.Type == filter.Type);
            }

            conditions.Add(n => n.ForUserId == null);

            IQueryable<SiteNotification> query = _context.SiteNotifications;

            foreach (var item in conditions ?? new FilterConditions<SiteNotification>())
            {
                query = query.Where(item);
            }

            query = query.OrderByDescending(n => n.Id);

            await filter.Paging(query.Select(n => new AdminSideNotificationDto()
            {
                Id = n.Id,
                Content = n.Content,
                CreatedDate = n.CreatedDate,
                IsRead = n.IsRead
            }));

            return filter;
        }

        public async Task<Result<ClientSideFilterNotificationsDto>> ClientSideFilterNotifications(ClientSideFilterNotificationsDto filter)
        {
            if (filter is null)
                return Result.Failure<ClientSideFilterNotificationsDto>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1)
                return Result.Failure<ClientSideFilterNotificationsDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<SiteNotification>();
            conditions.Add(n => n.ForUserId == userId);

            IQueryable<SiteNotification> query = _context.SiteNotifications;

            foreach (var item in conditions ?? new FilterConditions<SiteNotification>())
            {
                query = query.Where(item);
            }

            query = query.OrderByDescending(n => n.Id);

            await filter.Paging(query.Select(n => new ClientSideNotificationDto()
            {
                Id = n.Id,
                Content = n.Content,
                CreatedDate = n.CreatedDate
            }));

            return filter;
        }

        public async Task<Result> AdminSideCreateNewNotification(AdminSideCreateNewNotificationDto model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            var notification = new SiteNotification()
            {
                Content = model.Content,
                ForUserId = null,
                IsRead = false,
                Type = model.Type,
            };

            _context.SiteNotifications.Add(notification);
            await _context.SaveChangesAsync();

            // Best-effort mirror to the Telegram channel — never blocks or fails
            // the admin request if the worker is down.
            _ = Task.Run(() => _telegramService.SendSiteNotificationToChannelAsync(notification.Content));

            return Result.Success();
        }

        public async Task<Result> ClientSideCreateNewNotification(ClientSideCreateNewNotificationDto model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            var notification = new SiteNotification()
            {
                Content = model.Content,
                ForUserId = model.UserId,
                IsRead = false,
            };

            _context.Add(notification);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<int> AdminSideGetUnreadNotificationsCount()
        {
            return await _context.SiteNotifications
                .AsNoTracking()
                .CountAsync(n => n.ForUserId == null && n.IsRead == false);
        }

        public async Task<int> ClientSideGetUnreadNotificationsCount()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1)
                return 0;

            return await _context.SiteNotifications
                .AsNoTracking()
                .CountAsync(n => n.ForUserId == userId && n.IsRead == false);
        }

        public Task AdminSideSetNotificationsRead()
        {
            return _context.SiteNotifications
                .Where(n => n.ForUserId == null && n.IsRead == false)
                .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.LastUpdatedDate, DateTime.UtcNow));
        }

        public Task ClientSideSetNotificationsRead()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1)
                return Task.CompletedTask;

            return _context.SiteNotifications
                .Where(n => n.ForUserId == userId && n.IsRead == false)
                .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.LastUpdatedDate, DateTime.UtcNow));
        }
    }
}
