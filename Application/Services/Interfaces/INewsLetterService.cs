using Application.Dtos.NewsLetter.AdminSide;
using Application.Dtos.NewsLetter.ClientSide;
using Application.Dtos.SentLetter.AdminSide;
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
