using Application.ViewModels.SiteNotifications.AdminSide;
using Application.ViewModels.SiteNotifications.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ISiteNotificationService
    {
        Task<Result<AdminSideFilterNotificationsViewModel>> AdminSideFilterNotifications(AdminSideFilterNotificationsViewModel filter);

        Task<Result<ClientSideFilterNotificationsViewModel>> ClientSideFilterNotifications(ClientSideFilterNotificationsViewModel filter);

        Task<Result> AdminSideCreateNewNotification(AdminSideCreateNewNotificationViewModel model);

        Task<Result> ClientSideCreateNewNotification(ClientSideCreateNewNotificationViewModel model);

        Task<Result<List<AdminSideNotificationViewModel>>> AdminSideGetLatestNotifications();
    }
}
