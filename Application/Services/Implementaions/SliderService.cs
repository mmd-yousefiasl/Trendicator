using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
using Application.Tools;
using Application.ViewModels.Slider;
using Application.ViewModels.Slider.AdminSide;
using Data.Context;
using Domain.Events.SiteInfo.Slider;
using Domain.Models.SiteInfo;
using Domain.Shared;
using Domain.Statics;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class SliderService : ISliderService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Slider> _dbSet;
        private readonly IMediatorHandler _mediator;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor
        public SliderService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            ICacheManager cacheManager)
        {
            _context = context;
            _dbSet = _context.Set<Slider>();
            _mediator = mediator;
            _cacheManager = cacheManager;
        }
        #endregion

        public async Task<Result<AdminSideFilterSlidersViewModel>> AdminSideFilterSldiers(AdminSideFilterSlidersViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterSlidersViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Slider>();
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.SubTitle.Contains(filter.Search) ||
                n.Title.Contains(filter.Search) ||
                n.ButtonTitle.Contains(filter.Search) ||
                n.Url.Contains(filter.Search));
            }

            if (filter.IsPublished is not null)
            {
                conditions.Add(n => n.IsPublished == filter.IsPublished);
            }

            IQueryable<Slider> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Slider>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideSliderViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                SubTitle = n.SubTitle,
                ButtonTitle = n.ButtonTitle,
                IsPublished = n.IsPublished,
                Priority = n.Priority,
            }));

            return filter;
        }

        public async Task<Result> AdminSideCreateNewSlider(AdminSideCreateNewSliderViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            if (!model.BannerFile.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
            if (model.BannerFile.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.BannerFile.FileName);
            model.BannerFile.UplaodFileToServer(fileName, SiteTools.SliderBanner);

            Slider slider = new Slider()
            {
                Banner = fileName,
                SubTitle = model.SubTitle,
                Title = model.Title,
                ButtonTitle = model.ButtonTitle,
                IsPublished = model.IsPublished,
                Priority = model.Priority,
                Url = model.Url
            };

            await _dbSet.AddAsync(slider);
            await _context.SaveChangesAsync();

            SliderCreatedEvent SliderCreatedEvent = new SliderCreatedEvent(
                slider.Banner,
                slider.SubTitle,
                slider.Title,
                slider.ButtonTitle,
                slider.Url,
                slider.Priority,
                slider.IsPublished);

            await _mediator.PublishEvent(SliderCreatedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideUpdateSliderViewModel>> AdminSideGetSliderForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateSliderViewModel>(Messages.OperationFailedError);

            var slider = await _dbSet.Where(n => n.Id == id)
                .Select(n => new AdminSideUpdateSliderViewModel()
                {
                    Id = n.Id,
                    Title = n.Title,
                    SubTitle = n.SubTitle,
                    BannerFileName = n.Banner,
                    ButtonTitle = n.ButtonTitle,
                    IsPublished = n.IsPublished,
                    Priority = n.Priority,
                    Url = n.Url,
                }).AsNoTracking().FirstOrDefaultAsync();
            if (slider is null) return Result.Failure<AdminSideUpdateSliderViewModel>(Messages.OperationFailedError);

            return slider;
        }

        public async Task<Result> AdminSideUpdateSlider(AdminSideUpdateSliderViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var slider = await _dbSet.FindAsync(model.Id);
            if (slider is null) return Result.Failure(Messages.OperationFailedError);

            if (model.BannerFile is not null)
            {
                if (!model.BannerFile.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
                if (model.BannerFile.Length > SiteTools.MaxImageFileLength.CalculateMB()) return Result.Failure(Messages.FileLengthError);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.BannerFile.FileName);
                model.BannerFile.UplaodFileToServer(fileName, SiteTools.SliderBanner);

                slider.Banner.RemoveFileFromServer(SiteTools.SliderBanner);

                slider.Banner = fileName;
            }

            slider.SubTitle = model.SubTitle;
            slider.Title = model.Title;
            slider.ButtonTitle = model.ButtonTitle;
            slider.Url = model.Url;
            slider.IsPublished = model.IsPublished;
            slider.Priority = model.Priority;

            _dbSet.Update(slider);
            await _context.SaveChangesAsync();

            SliderUpdatedEvent SliderUpdatedEvent = new SliderUpdatedEvent(
            slider.Id,
            slider.Banner,
            slider.SubTitle,
            slider.Title,
            slider.ButtonTitle,
            slider.Url,
            slider.Priority,
            slider.IsPublished);

            await _mediator.PublishEvent(SliderUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteSlider(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var slider = await _dbSet.FindAsync(id);
            if (slider is null) return Result.Failure(Messages.OperationFailedError);

            slider.Banner.RemoveFileFromServer(SiteTools.SliderBanner);

            _dbSet.Remove(slider);
            await _context.SaveChangesAsync();

            SliderDeletedEvent SliderDeletedEvent = new SliderDeletedEvent(
                slider.Id);

            await _mediator.PublishEvent(SliderDeletedEvent);

            return Result.Success();
        }

        public async Task<Result<List<ClientSideSliderViewModel>>> ClientSideGetMainPageSliders()
        {
            var sliders = await _cacheManager.GetAsync(CacheKeys.SliderCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.IsPublished)
                .AsNoTracking().ToListAsync();
            });
            if (!sliders?.Any() ?? false) return Result.Failure<List<ClientSideSliderViewModel>>(Messages.OperationFailedError);

            return sliders.OrderBy(n => n.Priority).Select(n => new ClientSideSliderViewModel()
            {
                Title = n.Title,
                Banner = n.Banner,
                SubTitle = n.SubTitle,
                ButtonTitle = n.ButtonTitle,
                Url = n.Url
            }).ToList();
        }
    }
}
