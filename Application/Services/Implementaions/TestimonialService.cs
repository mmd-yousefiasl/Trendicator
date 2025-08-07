using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.Testimonial.AdminSide;
using Application.ViewModels.Testimonial.ClientSide;
using Data.Context;
using Domain.Events.SiteInfo.Testimonial;
using Domain.Models.SiteInfo;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Net.WebSockets;

namespace Application.Services.Implementaions
{
    public class TestimonialService : ITestimonialService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Testimonial> _dbSet;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public TestimonialService(TrendicatorDbContext context,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<Testimonial>();
            _mediator = mediator;
        }
        #endregion

        public async Task<Result<AdminSideFilterTestimonialsViewModel>> AdminSideFilterTestimonials(AdminSideFilterTestimonialsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterTestimonialsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Testimonial>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Content.Contains(filter.Search) ||
                n.Writer.Contains(filter.Search) ||
                n.Position.Contains(filter.Search));
            }

            if (filter.IsPublished is not null)
            {
                conditions.Add(n => n.IsPublished == filter.IsPublished);
            }

            IQueryable<Testimonial> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Testimonial>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideTestimonialViewModel()
            {
                Id = n.Id,
                IsPublished = n.IsPublished,
                Position = n.Position,
                Priority = n.Priority,
                Writer = n.Writer
            }));

            return filter;
        }

        public async Task<Result> AdminSideCreateNewTestimonial(AdminSideCreateNewTestimonialViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            Testimonial testimonial = new Testimonial()
            {
                Content = model.Content,
                Writer = model.Writer,
                IsPublished = model.IsPublished,
                Priority = model.Priority,
                Position = model.Position
            };

            await _dbSet.AddAsync(testimonial);
            await _context.SaveChangesAsync();

            TestimonialCreatedEvent TestimonialCreatedEvent = new TestimonialCreatedEvent(
                testimonial.Content,
                testimonial.Writer,
                testimonial.Position,
                testimonial.Priority,
                testimonial.IsPublished);

            await _mediator.PublishEvent(TestimonialCreatedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideUpdateTestimonialViewModel>> AdminSideGetTestimonialForUpdate(int id)
        {
            if (id < 1) return Result.Failure<AdminSideUpdateTestimonialViewModel>(Messages.OperationFailedError);

            var testimonial = await _dbSet.Where(n => n.Id == id).Select(n => new AdminSideUpdateTestimonialViewModel()
            {
                Id = n.Id,
                Content = n.Content,
                Writer = n.Writer,
                IsPublished = n.IsPublished,
                Position = n.Position,
                Priority = n.Priority
            }).AsNoTracking().FirstOrDefaultAsync();
            if (testimonial is null) return Result.Failure<AdminSideUpdateTestimonialViewModel>(Messages.OperationFailedError);

            return testimonial;
        }

        public async Task<Result> AdminSideUpdateTestimonial(AdminSideUpdateTestimonialViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var testimonial = await _dbSet.FindAsync(model.Id);
            if (testimonial is null) return Result.Failure(Messages.OperationFailedError);

            testimonial.Content = model.Content;
            testimonial.Writer = model.Writer;
            testimonial.Position = model.Position;
            testimonial.Priority = model.Priority;
            testimonial.IsPublished = model.IsPublished;

            _dbSet.Update(testimonial);
            await _context.SaveChangesAsync();

            TestimonialUpdatedEvent TestimonialUpdatedEvent = new TestimonialUpdatedEvent(
            testimonial.Id,
            testimonial.Content,
            testimonial.Writer,
            testimonial.Position,
            testimonial.Priority,
            testimonial.IsPublished);

            await _mediator.PublishEvent(TestimonialUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> AdminSideDeleteTestimonial(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var testimonial = await _dbSet.FindAsync(id);
            if (testimonial is null) return Result.Failure(Messages.OperationFailedError);

            _dbSet.Remove(testimonial);
            await _context.SaveChangesAsync();

            TestimonialDeletedEvent TestimonialDeletedEvent = new TestimonialDeletedEvent(
                testimonial.Id);

            await _mediator.PublishEvent(TestimonialDeletedEvent);

            return Result.Success();
        }

        public async Task<Result<List<ClientSideTestimonialViewModel>>> ClientSideGetTestimonials()
        {
            var testimonials = await _dbSet.Where(n => n.IsPublished).OrderBy(n => n.Priority)
                .Select(n => new ClientSideTestimonialViewModel()
                {
                    Content = n.Content,
                    Writer = n.Writer,
                    Position = n.Position
                }).AsNoTracking().ToListAsync();
            if (!testimonials.Any()) return Result.Failure<List<ClientSideTestimonialViewModel>>(Messages.OperationFailedError);

            return testimonials;
        }
    }
}
