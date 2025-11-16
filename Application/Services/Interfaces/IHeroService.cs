using Application.ViewModels.Hero.AdminSide;
using Application.ViewModels.Hero.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IHeroService
    {
        Task<Result<ClientSideHeroViewModel>> ClientSideGetHero();

        Task<Result<AdminSideUpdateHeroViewModel>> AdminSideGetHeroForUpdate();

        Task<Result> AdminSideUpdateHero(AdminSideUpdateHeroViewModel model);
    }
}
