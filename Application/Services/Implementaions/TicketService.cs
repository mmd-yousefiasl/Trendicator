using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Application.ViewModels.Ticket.AdminSide;
using Application.ViewModels.Ticket.ClientSide;
using Data.Context;
using Domain.Enums.Tickets;
using Domain.Events.Tickets;
using Domain.Models.Tickets;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class TicketService : ITicketService
    {
        #region Fields
        private readonly TrendicatorDbContext _context;
        private protected DbSet<Ticket> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public TicketService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMediatorHandler mediator)
        {
            _context = context;
            _dbSet = _context.Set<Ticket>();
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }
        #endregion

        public async Task<Result<ClientSideFilterTicketsViewModel>> ClientSideFilterTickets(ClientSideFilterTicketsViewModel filter)
        {
            if (filter is null) return Result.Failure<ClientSideFilterTicketsViewModel>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<ClientSideFilterTicketsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Ticket>();
            conditions.Add(n => n.UserId == userId);

            IQueryable<Ticket> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Ticket>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new ClientSideTicketViewModel()
            {
                Id = n.Id,
                CreatedDate = n.CreatedDate,
                LastUpdatedDate = n.LastUpdatedDate,
                Status = n.Status,
                Subject = n.Subject
            }));

            return filter;
        }

        public async Task<Result> ClientSideCreateNewTicket(ClientSideCreateNewTicketViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            var ticket = new Ticket()
            {
                Subject = model.Subject,
                UserId = userId,
                Status = TicketStatusEnum.Sent,
                Messages = new List<TicketMessage>()
            };

            ticket.Messages.Add(new TicketMessage()
            {
                Message = model.Message,
                IsSentByUser = true,
            });

            await _dbSet.AddAsync(ticket);
            await _context.SaveChangesAsync();

            TicketCreatedEvent TicketCreatedEvent = new TicketCreatedEvent(
                ticket.Subject,
                ticket.Status,
                ticket.UserId);

            await _mediator.PublishEvent(TicketCreatedEvent);

            return Result.Success(Messages.TicketSentSuccessfully);
        }

        public async Task<Result<ClientSideTicketDetailsViewModel>> ClientSideGetTicketDetails(int id)
        {
            if (id < 1) return Result.Failure<ClientSideTicketDetailsViewModel>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

            var ticket = await _dbSet.Where(n => n.Id == id && n.UserId == userId)
                .Select(n => new ClientSideTicketDetailsViewModel()
                {
                    Id = n.Id,
                    LastUpdatedDate = n.LastUpdatedDate,
                    Status = n.Status,
                    Subject = n.Subject,
                    Messages = n.Messages.OrderByDescending(n => n.Id).Select(i => new ClientSideTicketMessageViewModel()
                    {
                        Message = i.Message,
                        IsSentByUser = i.IsSentByUser,
                        CreatedDate = i.CreatedDate
                    }).ToList()
                }).AsNoTracking().FirstOrDefaultAsync();
            if (ticket is null) return Result.Failure<ClientSideTicketDetailsViewModel>(Messages.OperationFailedError);

            return ticket;
        }

        public async Task<Result> ClientSideCreateNewMessage(ClientSideCreateNewTicketMessageViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            var ticket = await _dbSet.Where(n => n.Id == model.TicketId && n.UserId == userId)
                .Include(n => n.Messages)
                .FirstOrDefaultAsync();
            if (ticket is null) return Result.Failure(Messages.OperationFailedError);

            ticket.Status = TicketStatusEnum.Sent;
            ticket.LastUpdatedDate = DateTime.Now;
            ticket.Messages.Add(new TicketMessage()
            {
                Message = model.Message,
                IsSentByUser = true,
            });

            _dbSet.Update(ticket);
            await _context.SaveChangesAsync();

            TicketMessageCreatedEvent TicketMessageCreatedEvent = new TicketMessageCreatedEvent(
                model.Message,
                true,
                ticket.Id);

            await _mediator.PublishEvent(TicketMessageCreatedEvent);

            return Result.Success();
        }

        public async Task<Result<AdminSideFilterTicketsViewModel>> AdminSideFilterTickets(AdminSideFilterTicketsViewModel filter)
        {
            if (filter is null) return Result.Failure<AdminSideFilterTicketsViewModel>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Ticket>();

            if (filter.Status is not null)
            {
                conditions.Add(n => n.Status == filter.Status);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Subject.Contains(filter.Search));
            }

            IQueryable<Ticket> query = _dbSet;
            query = query.OrderByDescending(n => n.Id);

            foreach (var item in conditions ?? new FilterConditions<Ticket>())
            {
                query = query.Where(item);
            }

            await filter.Paging(query.Select(n => new AdminSideTicketViewModel()
            {
                Id = n.Id,
                Status = n.Status,
                Subject = n.Subject,
                CreatedDate = n.CreatedDate,
                LastUpdatedDate = n.LastUpdatedDate,
                UserId = n.UserId,
                UserName = n.User.FirstName + " " + n.User.LastName
            }));

            return filter;
        }

        public async Task<Result<AdminSideTicketDetailsViewModel>> AdminSideGetTicketDetails(int id)
        {
            if (id < 1) return Result.Failure<AdminSideTicketDetailsViewModel>(Messages.OperationFailedError);

            var ticket = await _dbSet.Where(n => n.Id == id)
                .Select(n => new AdminSideTicketDetailsViewModel()
                {
                    Id = n.Id,
                    Status = n.Status,
                    Subject = n.Subject,
                    LastUpdatedDate = n.LastUpdatedDate,
                    UserName = n.User.FirstName + " " + n.User.LastName,
                    Messages = n.Messages.OrderByDescending(n => n.Id).Select(m => new AdminSideTicketMessageViewModel()
                    {
                        Message = m.Message,
                        IsSentByUser = m.IsSentByUser,
                        CreatedDate = m.CreatedDate
                    }).ToList()
                }).AsNoTracking().FirstOrDefaultAsync();
            if (ticket is null) return Result.Failure<AdminSideTicketDetailsViewModel>(Messages.OperationFailedError);

            return ticket;
        }

        public async Task<Result> AdminSideCreateNewMessage(AdminSideCreateNewTicketMessageViewModel model)
        {
            if (model is null) return Result.Failure(Messages.OperationFailedError);

            var ticket = await _dbSet.Where(n => n.Id == model.TicketId).Include(n => n.Messages)
                .FirstOrDefaultAsync();
            if (ticket is null) return Result.Failure(Messages.OperationFailedError);

            ticket.LastUpdatedDate = DateTime.Now;
            ticket.Status = TicketStatusEnum.Answered;
            ticket.Messages.Add(new TicketMessage()
            {
                Message = model.Message,
                IsSentByUser = false,
            });

            _dbSet.Update(ticket);
            await _context.SaveChangesAsync();

            TicketMessageCreatedEvent TicketMessageCreatedEvent = new TicketMessageCreatedEvent(
                model.Message,
                false,
                ticket.Id);

            await _mediator.PublishEvent(TicketMessageCreatedEvent);

            return Result.Success();
        }

        public async Task<Result> SoftDeleteTicket(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var ticket = await _dbSet.FindAsync(id);
            if (ticket is null) return Result.Failure(Messages.OperationFailedError);

            ticket.IsDeleted = true;
            ticket.LastUpdatedDate = DateTime.Now;

            _dbSet.Update(ticket);
            await _context.SaveChangesAsync();

            TicketDeletedEvent TicketDeletedEvent = new TicketDeletedEvent(
                ticket.Id);

            await _mediator.PublishEvent(TicketDeletedEvent);

            return Result.Success();
        }

        public async Task<Result> AdminSideCloseTicket(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var ticket = await _dbSet.FindAsync(id);
            if (ticket is null) return Result.Failure(Messages.OperationFailedError);

            ticket.Status = TicketStatusEnum.Closed;
            ticket.LastUpdatedDate = DateTime.Now;

            _dbSet.Update(ticket);
            await _context.SaveChangesAsync();

            TicketUpdatedEvent TicketUpdatedEvent = new TicketUpdatedEvent(
                ticket.Id,
                ticket.Subject,
                ticket.Status,
                ticket.UserId);

            await _mediator.PublishEvent(TicketUpdatedEvent);

            return Result.Success();
        }

        public async Task<Result> ClientSideClostTicket(int id)
        {
            if (id < 1) return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure(Messages.OperationFailedError);

            var ticket = await _dbSet.Where(n => n.Id == id && n.UserId == userId).FirstOrDefaultAsync();
            if (ticket is null) return Result.Failure(Messages.OperationFailedError);

            ticket.Status = TicketStatusEnum.Closed;
            ticket.LastUpdatedDate = DateTime.Now;

            _dbSet.Update(ticket);
            await _context.SaveChangesAsync();

            TicketUpdatedEvent TicketUpdatedEvent = new TicketUpdatedEvent(
                ticket.Id,
                ticket.Subject,
                ticket.Status,
                ticket.UserId);

            await _mediator.PublishEvent(TicketUpdatedEvent);

            return Result.Success();
        }

        public async Task<int> MyTicketsCount()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return 0;

            return await _dbSet.Where(n => n.UserId == userId).CountAsync();
        }
    }
}
