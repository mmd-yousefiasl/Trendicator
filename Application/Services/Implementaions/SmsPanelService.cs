using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.SmsData.AdminSide;
using Application.ViewModels.SmsData.ClientSide;
using Data.Context;
using Domain.Enums.SiteInfo;
using Domain.Models.SiteInfo;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class SmsDataService : ISmsPanelService
    {
        #region Fields
        private protected TrendicatorDbContext _context;
        #endregion

        #region Ctor
        public SmsDataService(TrendicatorDbContext context)
        {
            _context = context;
        }
        #endregion

        public async Task<Result> AdminSideCreateNewSmsPanel(AdminSideCreateNewSmsPanelDto dto)
        {
            if (dto is null)
                return Result.Failure(Messages.OperationFailedError);

            if (dto.IsActive)
            {
                var currentlyActivePanel = await _context.SmsPanels
                    .FirstOrDefaultAsync(n => n.Type == dto.Type && n.IsActive);

                if (currentlyActivePanel != null)
                {
                    currentlyActivePanel.IsActive = false;
                }
            }

            var newPanel = new SmsPanel
            {
                ApiKey = dto.ApiKey,
                IsActive = dto.IsActive,
                PatternId = dto.PatternId,
                Type = dto.Type,
            };

            _context.SmsPanels.Add(newPanel);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteSmsPanel(int id)
        {
            if (id < 1)
                return Result.Failure(Messages.OperationFailedError);

            var panel = await _context.SmsPanels.FindAsync(id);
            if (panel is null)
                return Result.Failure(Messages.OperationFailedError);

            panel.IsDeleted = true;

            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterSmsPanelsDto>> AdminSideFilterSmsPanles(AdminSideFilterSmsPanelsDto filter)
        {
            if (filter is null)
                return Result.Failure<AdminSideFilterSmsPanelsDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<SmsPanel>();

            if (filter.Type is not null)
            {
                conditions.Add(n => n.Type == filter.Type);
            }

            if (filter.IsActive is not null)
            {
                conditions.Add(n => n.IsActive == filter.IsActive);
            }

            IQueryable<SmsPanel> query = _context.SmsPanels;

            foreach (var item in conditions ?? new FilterConditions<SmsPanel>())
            {
                query = query.Where(item);
            }

            query = query.OrderByDescending(n => n.Id);

            await filter.Paging(query.Select(n => new AdminSideSmsPanelDto()
            {
                Id = n.Id,
                IsActive = n.IsActive,
                Type = n.Type
            }));

            return filter;
        }

        public async Task<Result<AdminSideUpdateSmsPanelDto>> AdminSideGetSmsPanelForUpdate(int id)
        {
            if (id < 1)
                return Result.Failure<AdminSideUpdateSmsPanelDto>(Messages.OperationFailedError);

            var panel = await _context.SmsPanels
                .AsNoTracking()
                .Where(n => n.Id == id)
                .Select(n => new AdminSideUpdateSmsPanelDto()
                {
                    Id = n.Id,
                    IsActive = n.IsActive,
                    ApiKey = n.ApiKey,
                    PatternId = n.PatternId,
                    Type = n.Type
                }).FirstOrDefaultAsync();
            if (panel is null)
                return Result.Failure<AdminSideUpdateSmsPanelDto>(Messages.OperationFailedError);

            return panel;
        }

        public async Task<Result> AdminSideUpdateSmsPanel(AdminSideUpdateSmsPanelDto dto)
        {
            if (dto is null)
                return Result.Failure(Messages.OperationFailedError);

            var panel = await _context.SmsPanels.FindAsync(dto.Id);
            if (panel is null)
                return Result.Failure(Messages.OperationFailedError);

            if (dto.IsActive)
            {
                var currentlyActivePanel = await _context.SmsPanels
                    .FirstOrDefaultAsync(n => n.Type == dto.Type && n.IsActive && n.Id != dto.Id);

                if (currentlyActivePanel != null)
                {
                    currentlyActivePanel.IsActive = false;
                }
            }

            panel.ApiKey = dto.ApiKey;
            panel.PatternId = dto.PatternId;
            panel.Type = dto.Type;
            panel.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<ClientSideSmsPanelDto>> ClientSideGetSmsPanel(SmsPanelTypeEnum type)
        {
            var panel = await _context.SmsPanels
                .AsNoTracking()
                .Where(n => n.Type == type && n.IsActive == true)
                .Select(n => new ClientSideSmsPanelDto()
                {
                    ApiKey = n.ApiKey,
                    PatternId = n.PatternId
                }).FirstOrDefaultAsync();
            if (panel is null)
                return Result.Failure<ClientSideSmsPanelDto>(Messages.OperationFailedError);

            return panel;
        }
    }
}
