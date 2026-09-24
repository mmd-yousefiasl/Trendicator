using Application.Dtos.ProductFaq.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IProductFaqService
    {
        Task<Result<AdminSideFilterProductFaqsViewModel>> AdminSideFilterProductFaqs(AdminSideFilterProductFaqsViewModel filter);

        Task<Result> AdminSideCreateNewProductFaq(AdminSideCreateNewProductFaqViewModel model);

        Task<Result<AdminSideUpdateProductFaqViewModel>> AdminSideGetProductFaqForUpdate(int id);

        Task<Result> AdminSideUpdateProductFaq(AdminSideUpdateProductFaqViewModel model);

        Task<Result> AdminSideDeleteProductFaq(int id);
    }
}
