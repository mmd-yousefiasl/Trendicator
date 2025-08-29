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
        private protected DbSet<SiteNotification> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Ctor
        public SiteNotificationService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _dbSet = _context.Set<SiteNotification>();
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        public async Task<Result<AdminSideFilterNotificationsViewModel>> AdminSideFilterNotifications(AdminSideFilterNotificationsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterNotificationsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<SiteNotification>();

            conditions.Add(n => n.ForUserId == null);

            IQueryable<SiteNotification> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<SiteNotification>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideNotificationViewModel()
            {
                Id = n.Id,
                Content = n.Content,
                CreatedDate = n.CreatedDate
            }));

            return filter;
        }

        public async Task<Result<ClientSideFilterNotificationsViewModel>> ClientSideFilterNotifications(ClientSideFilterNotificationsViewModel filter)
        {
            if (filter is null) return Result.Failure<ClientSideFilterNotificationsViewModel>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<ClientSideFilterNotificationsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<SiteNotification>();
            conditions.Add(n => n.ForUserId == userId);

            IQueryable<SiteNotification> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<SiteNotification>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideNotificationViewModel()
            {
                Id = n.Id,
                Content = n.Content,
                CreatedDate = n.CreatedDate
            }));

            return filter;
        }

        public async Task<Result> AdminSideCreateNewNotification(AdminSideCreateNewNotificationViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var notification = new SiteNotification()
            {
                Content = model.Content,
                ForUserId = null,
            };

            await _dbSet.AddAsync(notification);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> ClientSideCreateNewNotification(ClientSideCreateNewNotificationViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var notification = new SiteNotification()
            {
                Content = model.Content,
                ForUserId = model.UserId,
            };

            await _dbSet.AddAsync(notification);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<List<AdminSideNotificationViewModel>>> AdminSideGetLatestNotifications()
        {
            var notifications = await _dbSet.Where(n => n.ForUserId == null)
                .OrderByDescending(n => n.Id).Take(5)
                .Select(n => new AdminSideNotificationViewModel()
                {
                    Id = n.Id,
                    Content = n.Content,
                    CreatedDate = n.CreatedDate
                }).AsNoTracking().ToListAsync();
            if (!notifications.Any()) return Result.Failure<List<AdminSideNotificationViewModel>>(Messages.OperationFailedError);

            return notifications;
        }
    }
}
