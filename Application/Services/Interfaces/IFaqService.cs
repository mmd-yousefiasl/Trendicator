using Application.Dtos.Faq.AdminSide;
using Application.Dtos.Faq.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IFaqService
    {
        Task<Result<AdminSideFilterFaqsViewModel>> FilterFaqs(AdminSideFilterFaqsViewModel filter);

        Task<Result> CreateNewFaq(AdminSideCreateNewFaqViewModel model);

        Task<Result<AdminSideUpdateFaqViewModel>> GetFaqForUpdate(int id);

        Task<Result> UpdateFaq(AdminSideUpdateFaqViewModel model);

        Task<Result> SoftDeleteFaq(int id);

        Task<Result<List<ClientSideFaqViewModel>>> GetFaqs();
    }
}
