using Application.Dtos.ContactUsForm.AdminSide;
using Application.Dtos.ContactUsForm.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Events.ContactUs;
using Domain.Models.ContactUs;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Implementaions
{
    public class ContactUsFormService : IContactUsFormService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public ContactUsFormService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMediatorHandler mediator)
        {
            _context = context;
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
                n.Mobile.Contains(filter.Search) ||
                n.UserIP.Contains(filter.Search));
            }

            if (filter.Type is not null)
            {
                conditions.Add(n => n.Type == filter.Type);
            }

            IQueryable<ContactUsForm> query = _context.ContactUsForms;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<ContactUsForm>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideContactUsFormViewModel()
            {
                Id = n.Id,
                CreatedDate = n.CreatedDate,
                FullName = n.FullName,
                Type = n.Type,
                Message = n.Message,
                Mobile = n.Mobile,
                UserId = n.UserId,
                UserIP = n.UserIP
            }));

            return filter;
        }

        public async Task<Result> SoftDeleteContactUsForm(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var form = await _context.ContactUsForms.FindAsync(id);
            if (form is null) return Result.Failure(Messages.OperationFailedError);

            form.IsDeleted = true;

            _context.ContactUsForms.Update(form);
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
                FullName = model.FullName,
                Type = model.Type,
                Mobile = model.Mobile,
                Message = model.Message,
                UserIP = userIP,
                CreatedDate = DateTime.UtcNow
            };
            if (userId > 0)
            {
                form.UserId = userId;
            }

            _context.ContactUsForms.Add(form);
            await _context.SaveChangesAsync();

            ContactUsFormCreatedEvent ContactUsFormCreatedEvent = new ContactUsFormCreatedEvent(
                form.CreatedDate,
                form.FullName,
                form.Mobile,
                form.Type,
                form.Message,
                form.UserId,
                form.UserIP);

            await _mediator.PublishEvent(ContactUsFormCreatedEvent);

            return Result.Success();
        }
    }
}
