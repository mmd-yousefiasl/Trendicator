using Application.Dtos.SmsData.AdminSide;
using Application.Dtos.SmsData.ClientSide;
using Domain.Enums.SiteInfo;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ISmsPanelService
    {
        Task<Result> AdminSideCreateNewSmsPanel(AdminSideCreateNewSmsPanelDto dto);

        Task<Result<AdminSideUpdateSmsPanelDto>> AdminSideGetSmsPanelForUpdate(int id);

        Task<Result> AdminSideUpdateSmsPanel(AdminSideUpdateSmsPanelDto dto);

        Task<Result<AdminSideFilterSmsPanelsDto>> AdminSideFilterSmsPanles(AdminSideFilterSmsPanelsDto filter);

        Task<Result> AdminSideDeleteSmsPanel(int id);

        Task<Result<ClientSideSmsPanelDto>> ClientSideGetSmsPanel(SmsPanelTypeEnum type);
    }
}
