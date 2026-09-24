using Application.Dtos.Hero.AdminSide;
using Application.Dtos.Hero.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
using Application.Tools;
using Data.Context;
using Domain.Events.SiteInfo;
using Domain.Models.SiteInfo;
using Domain.Shared;
using Domain.Statics;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class HeroService : IHeroService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Hero> _dbSet;
        private readonly ICacheManager _cacheManager;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public HeroService(TrendicatorDbContext context,
            ICacheManager cacheManager,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<Hero>();
            _cacheManager = cacheManager;
            _mediator = mediator;
        }
        #endregion

        public async Task<Result<ClientSideHeroViewModel>> ClientSideGetHero()
        {
            var hero = await _cacheManager.GetAsync(CacheKeys.HeroCacheKey, async () =>
            {
                return await _dbSet.Where(n => n.Id == 1).Select(n => new ClientSideHeroViewModel
                {
                    Title = n.Title,
                    Description = n.Description,
                    FileName = n.FileName
                }).AsNoTracking().FirstOrDefaultAsync();
            });

            if (hero is null) return Result.Failure<ClientSideHeroViewModel>(Messages.OperationFailedError);

            return hero;
        }

        public async Task<Result<AdminSideUpdateHeroViewModel>> AdminSideGetHeroForUpdate()
        {
            var hero = await _dbSet.Where(n => n.Id == 1).Select(n => new AdminSideUpdateHeroViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                Description = n.Description,
                FileName = n.FileName
            }).AsNoTracking().FirstOrDefaultAsync();
            if (hero is null) return Result.Failure<AdminSideUpdateHeroViewModel>(Messages.OperationFailedError);

            return hero;
        }

        public async Task<Result> AdminSideUpdateHero(AdminSideUpdateHeroViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var hero = await _dbSet.FindAsync(1);
            if (hero is null) return Result.Failure(Messages.OperationFailedError);

            hero.Title = model.Title;
            hero.Description = model.Description;

            if (model.File is not null)
            {
                if (!model.File.IsImage()) return Result.Failure(Messages.InvalidFileFormatError);
                if (model.File.Length > 5.CalculateMB()) return Result.Failure(Messages.FileLengthError);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
                model.File.UploadFileToServer(fileName, SiteTools.Banner);
                hero.FileName?.RemoveFileFromServer(SiteTools.Banner);

                hero.FileName = fileName;
            }

            _dbSet.Update(hero);
            await _context.SaveChangesAsync();

            HeroUpdatedEvent HeroUpdatedEvent = new HeroUpdatedEvent(
                hero.Id,
                hero.Title,
                hero.Description,
                hero.FileName);

            await _mediator.PublishEvent(HeroUpdatedEvent);

            return Result.Success();
        }
    }
}
