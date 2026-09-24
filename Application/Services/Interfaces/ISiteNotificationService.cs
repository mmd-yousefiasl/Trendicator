using Application.Dtos.SiteNotifications.AdminSide;
using Application.Dtos.SiteNotifications.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ISiteNotificationService
    {
        Task<Result<AdminSideFilterNotificationsDto>> AdminSideFilterNotifications(AdminSideFilterNotificationsDto filter);

        Task<Result<ClientSideFilterNotificationsDto>> ClientSideFilterNotifications(ClientSideFilterNotificationsDto filter);

        Task<Result> AdminSideCreateNewNotification(AdminSideCreateNewNotificationDto model);

        Task<Result> ClientSideCreateNewNotification(ClientSideCreateNewNotificationDto model);

        Task<int> AdminSideGetUnreadNotificationsCount();

        Task<int> ClientSideGetUnreadNotificationsCount();

        Task AdminSideSetNotificationsRead();

        Task ClientSideSetNotificationsRead();
    }
}
