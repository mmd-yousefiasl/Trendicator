using Application.ViewModels.NewsLetter.AdminSide;
using Application.ViewModels.NewsLetter.ClientSide;
using Application.ViewModels.SentLetter.AdminSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface INewsLetterService
    {
        Task<Result> SignInNewsLetters(ClientSideSignInNewsLettersViewModel model);

        Task<Result<AdminSideFilterNewsLettersViewModel>> FilterNewsLetters(AdminSideFilterNewsLettersViewModel filter);

        Task<Result> SoftDeleteNewsLetter(int id);

        Task<Result> SendNews(AdminSideSendLetterViewModel model);

        Task<Result<List<string>>> GetNewsLetterEmails();
    }
}
