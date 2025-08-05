using Application.ViewModels.Slider;
using Application.ViewModels.Slider.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ISliderService
    {
        Task<Result<AdminSideFilterSlidersViewModel>> AdminSideFilterSldiers(AdminSideFilterSlidersViewModel filter);

        Task<Result> AdminSideCreateNewSlider(AdminSideCreateNewSliderViewModel model);

        Task<Result<AdminSideUpdateSliderViewModel>> AdminSideGetSliderForUpdate(int id);

        Task<Result> AdminSideUpdateSlider(AdminSideUpdateSliderViewModel model);

        Task<Result> AdminSideDeleteSlider(int id);

        Task<Result<List<ClientSideSliderViewModel>>> ClientSideGetMainPageSliders();
    }
}
