using Application.Dtos.Faq.AdminSide;
using Application.Dtos.Faq.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Static;
using Application.Tools;
using Data.Context;
using Domain.Events.Faqs;
using Domain.Models.Faqs;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class FaqService : IFaqService
    {
        #region Fields

        private readonly TrendicatorDbContext _context;
        private readonly IMediatorHandler _mediator;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public FaqService(TrendicatorDbContext context,
            IMediatorHandler mediator,
            ICacheManager cacheManager)
        {
            _context = context;
            _mediator = mediator;
            _cacheManager = cacheManager;
        }

        #endregion

        public async Task<Result> CreateNewFaq(AdminSideCreateNewFaqViewModel model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            Faq faq = new Faq()
            {
                Question = model.Question,
                Answer = model.Answer,
                Priority = model.Priority,
                IsPublished = model.IsPublished
            };

            _context.Faqs.Add(faq);
            await _context.SaveChangesAsync();

            FaqCreatedEvent FaqCreatedEvent = new FaqCreatedEvent(
                faq.Question,
                faq.Answer,
                faq.Priority,
                faq.IsPublished);

            await _mediator.PublishEvent(FaqCreatedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterFaqsViewModel>> FilterFaqs(AdminSideFilterFaqsViewModel filter)
        {
            if (filter is null)
                return Result.Failure<AdminSideFilterFaqsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Faq>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Question.Contains(filter.Search) || n.Answer.Contains(filter.Search));
            }

            if (filter.IsPublished is not null)
            {
                conditions.Add(n => n.IsPublished == filter.IsPublished);
            }

            IQueryable<Faq> query = _context.Faqs;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Faq>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideFaqViewModel()
            {
                Id = n.Id,
                Question = n.Question,
                Answer = n.Answer,
                Priority = n.Priority,
                IsPublished = n.IsPublished
            }));

            return filter;
        }

        public async Task<Result<AdminSideUpdateFaqViewModel>> GetFaqForUpdate(int id)
        {
            if (id < 1)
                return Result.Failure<AdminSideUpdateFaqViewModel>(Messages.OperationFailedError);

            var faq = await _context.Faqs
                .AsNoTracking()
                .Select(n => new AdminSideUpdateFaqViewModel()
                {
                    Id = n.Id,
                    Question = n.Question,
                    Answer = n.Answer,
                    IsPublished = n.IsPublished,
                    Priority = n.Priority
                }).FirstOrDefaultAsync();
            if (faq is null) return Result.Failure<AdminSideUpdateFaqViewModel>(Messages.OperationFailedError);

            return faq;
        }

        public async Task<Result<List<ClientSideFaqViewModel>>> GetFaqs()
        {
            var faqs = await _cacheManager.GetAsync(CacheKeys.FaqCacheKey, async () =>
            {
                return await _context.Faqs.AsNoTracking()
                    .Where(n => n.IsPublished)
                    .ToListAsync();
            });

            if (!faqs?.Any() ?? true)
                return Result.Failure<List<ClientSideFaqViewModel>>(Messages.OperationFailedError);

            return faqs.OrderBy(n => n.Priority).Select(n => new ClientSideFaqViewModel()
            {
                Answer = n.Answer,
                Question = n.Question,
                Priority = n.Priority
            }).ToList();
        }

        public async Task<Result<List<ClientSideFaqViewModel>>> ClientSideGetFaqsForMainPage(int take)
        {
            if (take < 1) return Result.Failure<List<ClientSideFaqViewModel>>(Messages.OperationFailedError);

            var faqs = await _cacheManager.GetAsync(CacheKeys.FaqCacheKey, async () =>
            {
                return await _context.Faqs
                    .AsNoTracking()
                    .OrderBy(n => n.Priority)
                    .Take(take)
                    .ToListAsync();
            });
            if (!faqs?.Any() ?? true)
                return Result.Failure<List<ClientSideFaqViewModel>>(Messages.OperationFailedError);

            return faqs.Select(n => new ClientSideFaqViewModel()
            {
                Question = n.Question,
                Answer = n.Answer,
                Priority = n.Priority
            }).ToList();
        }

        public async Task<Result> SoftDeleteFaq(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var faq = await _context.Faqs.FindAsync(id);
            if (faq is null) return Result.Failure(Messages.OperationFailedError);

            faq.IsDeleted = true;

            _context.Faqs.Update(faq);
            await _context.SaveChangesAsync();

            FaqDeletedEvent FaqDeletedEvent = new FaqDeletedEvent(
                faq.Id);

            await _mediator.PublishEvent(FaqDeletedEvent);

            return Result.Success();
        }

        public async Task<Result> UpdateFaq(AdminSideUpdateFaqViewModel model)
        {
            if (model is null)
                return Result.Failure(Messages.OperationFailedError);

            var faq = await _context.Faqs.FindAsync(model.Id);

            if (faq is null)
                return Result.Failure(Messages.OperationFailedError);

            faq.Question = model.Question;
            faq.Answer = model.Answer;
            faq.Priority = model.Priority;
            faq.IsPublished = model.IsPublished;

            _context.Faqs.Update(faq);
            await _context.SaveChangesAsync();

            FaqUpdatedEvent FaqUpdatedEvent = new FaqUpdatedEvent(
                faq.Id,
                faq.Question,
                faq.Answer,
                faq.Priority,
                faq.IsPublished);

            await _mediator.PublishEvent(FaqUpdatedEvent);

            return Result.Success();
        }
    }
}