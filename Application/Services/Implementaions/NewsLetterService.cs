using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.NewsLetter.AdminSide;
using Application.ViewModels.NewsLetter.ClientSide;
using Application.ViewModels.SendEmail;
using Application.ViewModels.SentLetter.AdminSide;
using Data.Context;
using Domain.Events.NewsLetters;
using Domain.Models.NewsLetters;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class NewsLetterService : INewsLetterService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<NewsLetter> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISentLetterService _sentLetterService;
        private readonly ISendEmailService _sendEmailService;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public NewsLetterService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ISentLetterService sentLetterService,
            ISendEmailService sendEmailService,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<NewsLetter>();
            _httpContextAccessor = httpContextAccessor;
            _sentLetterService = sentLetterService;
            _sendEmailService = sendEmailService;
            _mediator = mediator;
        }
        #endregion

        public async Task<Result> SignInNewsLetters(ClientSideSignInNewsLettersViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            if (await _dbSet.AnyAsync(n => n.Email == model.Email)) return Result.Failure(Messages.ThisEmailAlreadyRegisterdError);

            NewsLetter newsLetter = new NewsLetter()
            {
                Email = model.Email,
                UserIP = _httpContextAccessor.HttpContext.GetUserIP(),
            };

            var userId = _httpContextAccessor.HttpContext.User?.GetUserId() ?? null;
            if (userId is null || userId is 0)
            {
                newsLetter.UserId = null;
            }
            else
            {
                newsLetter.UserId = userId;
            }

            await _dbSet.AddAsync(newsLetter);
            await _context.SaveChangesAsync();

            NewsLetterCreatedEvent NewsLetterCreatedEvent = new NewsLetterCreatedEvent(
                newsLetter.Email,
                newsLetter.UserIP,
                newsLetter.UserId);

            await _mediator.PublishEvent(NewsLetterCreatedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterNewsLettersViewModel>> FilterNewsLetters(AdminSideFilterNewsLettersViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterNewsLettersViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<NewsLetter>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Email.Contains(filter.Search) ||
                n.UserIP.Contains(filter.Search));
            }

            IQueryable<NewsLetter> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<NewsLetter>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideNewsLetterViewModel()
            {
                Id = n.Id,
                Email = n.Email,
                UserIP = n.UserIP
            }));

            return filter;
        }

        public async Task<Result> SoftDeleteNewsLetter(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var newsLetter = await _dbSet.FindAsync(id);
            if (newsLetter is null) return Result.Failure(Messages.OperationFailedError);

            newsLetter.IsDeleted = true;
            newsLetter.LastUpdatedDate = DateTime.UtcNow;

            _dbSet.Update(newsLetter);
            await _context.SaveChangesAsync();

            NewsLetterDeletedEvent NewsLetterDeletedEvent = new NewsLetterDeletedEvent(
                newsLetter.Id);

            await _mediator.PublishEvent(NewsLetterDeletedEvent);

            return Result.Success();
        }

        public async Task<Result> SendNews(AdminSideSendLetterViewModel model)
        {
            var emailsResult = await GetNewsLetterEmails();
            if (emailsResult.IsFailure) return Result.Failure(emailsResult.Message);

            var sendBuckEmailResult = await _sendEmailService.SendBuckEmail(new SendBuckEmailViewModel()
            {
                Subject = model.Subject,
                Body = model.Message,
                Recipients = emailsResult.Value
            });

            await _sentLetterService.CreateNewSentLetter(new AdminSideCreateNewSentLetterViewModel()
            {
                Subject = model.Subject,
                IsSucceed = sendBuckEmailResult.IsSuccess,
                Message = model.Message,
                TargetMailCount = emailsResult.Value.Count()
            });

            if (sendBuckEmailResult.IsFailure) return Result.Failure(sendBuckEmailResult.Message);

            return Result.Success();
        }

        public async Task<Result<List<string>>> GetNewsLetterEmails()
        {
            var emails = await _dbSet.Select(n => n.Email).AsNoTracking().ToListAsync();
            if (!emails?.Any() ?? true) return Result.Failure<List<string>>(Messages.OperationFailedError);

            return emails;
        }
    }
}
