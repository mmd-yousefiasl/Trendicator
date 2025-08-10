using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.ContactUsForm.AdminSide;
using Application.ViewModels.ContactUsForm.ClientSide;
using Data.Context;
using Domain.Events.ContactUs;
using Domain.Models.ContactUs;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class ContactUsFormService : IContactUsFormService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<ContactUsForm> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public ContactUsFormService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<ContactUsForm>();
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }
        #endregion

        public async Task<Result<AdminSideFilterContactUsFormsViewModel>> FilterContactUsForms(AdminSideFilterContactUsFormsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterContactUsFormsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<ContactUsForm>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.FullName.Contains(filter.Search) ||
                n.Email.Contains(filter.Search) ||
                n.PhoneNumber.Contains(filter.Search) ||
                n.Subject.Contains(filter.Search) ||
                n.UserIP.Contains(filter.Search));
            }

            IQueryable<ContactUsForm> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<ContactUsForm>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideContactUsFormViewModel()
            {
                Id = n.Id,
                Email = n.Email,
                FullName = n.FullName,
                Subject = n.Subject,
                Message = n.Message,
                PhoneNumber = n.PhoneNumber,
                UserId = n.UserId,
                UserIP = n.UserIP
            }));

            return filter;
        }

        public async Task<Result> SoftDeleteContactUsForm(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var form = await _dbSet.FindAsync(id);
            if (form is null) return Result.Failure(Messages.OperationFailedError);

            form.IsDeleted = true;

            _dbSet.Update(form);
            await _context.SaveChangesAsync();

            ContactUsFormDeletedEvent ContactUsFormDeletedEvent = new ContactUsFormDeletedEvent(
                form.Id);

            await _mediator.PublishEvent(ContactUsFormDeletedEvent);

            return Result.Success();
        }

        public async Task<Result> UserContactUs(ClientSideContactUsViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var userIP = _httpContextAccessor.HttpContext.GetUserIP();

            ContactUsForm form = new ContactUsForm()
            {
                Email = model.Email,
                FullName = model.FullName,
                Subject = model.Subject,
                PhoneNumber = model.PhoneNumber,
                Message = model.Message,
                UserIP = userIP
            };
            if (userId > 0)
            {
                form.UserId = userId;
            }

            await _dbSet.AddAsync(form);
            await _context.SaveChangesAsync();

            ContactUsFormCreatedEvent ContactUsFormCreatedEvent = new ContactUsFormCreatedEvent(
                form.FullName,
                form.Email,
                form.PhoneNumber,
                form.Subject,
                form.Message,
                form.UserId,
                form.UserIP);

            await _mediator.PublishEvent(ContactUsFormCreatedEvent);

            return Result.Success();
        }
    }
}
