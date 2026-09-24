using Application.Dtos.ContactUsForm.AdminSide;
using Application.Dtos.ContactUsForm.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface IContactUsFormService
    {
        Task<Result<AdminSideFilterContactUsFormsViewModel>> FilterContactUsForms(AdminSideFilterContactUsFormsViewModel filter);

        Task<Result> SoftDeleteContactUsForm(int id);

        Task<Result> UserContactUs(ClientSideContactUsViewModel model);
    }
}
