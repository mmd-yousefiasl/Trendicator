using Application.Dtos.Tickets;
using Application.Dtos.Tickets.AdminSide;
using Application.Dtos.Tickets.ClientSide;
using Application.Mediator;
using Application.Services.Interfaces;
using Application.Tools;
using Data.Context;
using Domain.Enums.Tickets;
using Domain.Events.Tickets;
using Domain.Models.Tickets;
using Domain.Shared;
using Domain.Statics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementaions
{
    public class TicketService : ITicketService
    {
        #region Fields
        private protected TrendicatorDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediatorHandler _mediator;
        #endregion

        #region Ctor
        public TicketService(TrendicatorDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMediatorHandler mediator)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }
        #endregion

        public async Task<Result<int>> ClientSideCreateNewTicket(ClientSideCreateNewTicketDto dto)
        {
            if (dto is null)
                return Result.Failure<int>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext?.User.GetUserId() ?? 0;
            if (userId < 1)
                return Result.Failure<int>(Messages.OperationFailedError);

            var ticket = new Ticket()
            {
                Title = dto.Title,
                Status = TicketStatusEnum.Open,
                Category = dto.Category,
                Priority = dto.Priority,
                UserId = userId,
                Messages = new List<TicketMessage>()
            };

            var ticketMessage = new TicketMessage()
            {
                IsAdminResponse = false,
                SenderId = userId,
                Text = dto.MessageText
            };

            string? fileName = null;
            if (dto.Attachment is not null)
            {
                if (!dto.Attachment.IsImage())
                    return Result.Failure<int>(Messages.InvalidFileFormatError);

                if (dto.Attachment.Length > SiteTools.MaxTicketAttachmentLength.CalculateKB())
                    return Result.Failure<int>(Messages.MaxLengthError);

                fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(dto.Attachment.FileName);
                ticketMessage.AttachmentPath = fileName;
            }

            ticket.Messages.Add(ticketMessage);

            int createdTicketId = 0;

            var strategy = _context.Database.CreateExecutionStrategy();

            try
            {
                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        if (dto.Attachment is not null && fileName is not null)
                        {
                            dto.Attachment.UploadSensitiveFileToServer(fileName, SiteTools.TicketAttachmentFile);
                        }

                        _context.Tickets.Add(ticket);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        createdTicketId = ticket.Id;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();

                        if (dto.Attachment is not null && fileName is not null)
                        {
                            fileName.RemoveSensitiveFileFromServer(SiteTools.TicketAttachmentFile);
                        }

                        throw;
                    }
                });
            }
            catch (Exception)
            {
                return Result.Failure<int>(Messages.OperationFailedError);
            }

            await CreateNewTicketHistory(new CreateTicketHistoryDto
            {
                TicketId = createdTicketId,
                Description = Messages.NewTicketCreatedByUser
            });

            TicketCreatedEvent TicketCreatedEvent = new TicketCreatedEvent(
                ticket.CreatedDate,
                ticket.Title,
                ticket.Category,
                ticket.Priority,
                ticket.Status);

            await _mediator.PublishEvent(TicketCreatedEvent);

            return createdTicketId;
        }

        public async Task<Result<ClientSideFilterTicketsDto>> ClientSideFilterTickets(ClientSideFilterTicketsDto filter)
        {
            if (filter is null)
                return Result.Failure<ClientSideFilterTicketsDto>(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1) return Result.Failure<ClientSideFilterTicketsDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Ticket>();

            conditions.Add(n => n.UserId == userId);
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search));
            }

            if (filter.Status is not null)
            {
                conditions.Add(n => n.Status == filter.Status);
            }

            if (filter.Category is not null)
            {
                conditions.Add(n => n.Category == filter.Category);
            }

            if (filter.Priority is not null)
            {
                conditions.Add(n => n.Priority == filter.Priority);
            }

            IQueryable<Ticket> query = _context.Tickets;

            foreach (var item in conditions)
            {
                query = query.Where(item);
            }
            query = query.OrderByDescending(n => n.LastUpdatedDate);

            await filter.Paging(query.Select(n => new ClientSideTicketDto()
            {
                Id = n.Id,
                Status = n.Status,
                Category = n.Category,
                CreatedDate = n.CreatedDate,
                LastUpdatedDate = n.LastUpdatedDate,
                Priority = n.Priority,
                Title = n.Title
            }));

            return filter;
        }

        public async Task<Result<ClientSideTicketDetailsDto>> ClientSideGetTicketDetails(int ticketId)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1)
                return Result.Failure<ClientSideTicketDetailsDto>(Messages.OperationFailedError);

            var ticket = await _context.Tickets
                .AsNoTracking()
                .Where(n => n.Id == ticketId && n.UserId == userId)
                .Select(n => new ClientSideTicketDetailsDto()
                {
                    Id = n.Id,
                    Category = n.Category,
                    CreatedDate = n.CreatedDate,
                    Status = n.Status,
                    Title = n.Title,
                    Messages = n.Messages
                    .OrderBy(m => m.Id)
                    .Select(m => new ClientSideTicketMessageDto()
                    {
                        Id = m.Id,
                        Text = m.Text,
                        CreatedDate = m.CreatedDate,
                        AttachmentPath = m.AttachmentPath,
                        IsAdminResponse = m.IsAdminResponse,
                    }).ToList()
                }).FirstOrDefaultAsync();
            if (ticket is null)
                return Result.Failure<ClientSideTicketDetailsDto>(Messages.OperationFailedError);

            return ticket;
        }

        public async Task<Result> ClientSideCloseTicket(int ticketId)
        {
            if (ticketId < 1)
                return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1)
                return Result.Failure(Messages.OperationFailedError);

            var ticket = await _context.Tickets.Where(n => n.Id == ticketId && n.UserId == userId).FirstOrDefaultAsync();
            if (ticket is null)
                return Result.Failure(Messages.OperationFailedError);

            ticket.Status = TicketStatusEnum.Closed;
            ticket.LastUpdatedDate = DateTime.UtcNow;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            // Track user closing the ticket
            await CreateNewTicketHistory(new CreateTicketHistoryDto
            {
                TicketId = ticketId,
                Description = Messages.TicketClosedByUser
            });

            return Result.Success(Messages.TicketClosedSuccessfully);
        }

        public async Task<Result> ClientSideSendNewMessage(ClientSideSendTicketMessageDto dto)
        {
            if (dto is null)
                return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext?.User.GetUserId() ?? 0;
            if (userId < 1)
                return Result.Failure(Messages.OperationFailedError);

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == dto.TicketId && t.UserId == userId);

            if (ticket is null)
                return Result.Failure(Messages.OperationFailedError);

            var ticketMessage = new TicketMessage()
            {
                TicketId = dto.TicketId,
                SenderId = userId,
                Text = dto.MessageText,
                IsAdminResponse = false,
                CreatedDate = DateTime.UtcNow
            };

            string? fileName = null;
            if (dto.Attachment is not null)
            {
                if (!dto.Attachment.IsImage())
                    return Result.Failure(Messages.InvalidFileFormatError);

                if (dto.Attachment.Length > SiteTools.MaxTicketAttachmentLength.CalculateKB())
                    return Result.Failure(Messages.MaxLengthError);

                fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(dto.Attachment.FileName);
                ticketMessage.AttachmentPath = fileName;
            }

            ticket.Status = TicketStatusEnum.UserReplied;
            ticket.LastUpdatedDate = DateTime.UtcNow;

            var strategy = _context.Database.CreateExecutionStrategy();

            try
            {
                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        if (dto.Attachment is not null && fileName is not null)
                        {
                            dto.Attachment.UploadSensitiveFileToServer(fileName, SiteTools.TicketAttachmentFile);
                        }

                        _context.TicketMessages.Add(ticketMessage);
                        _context.Tickets.Update(ticket);

                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();

                        if (dto.Attachment is not null && fileName is not null)
                        {
                            fileName.RemoveSensitiveFileFromServer(SiteTools.TicketAttachmentFile);
                        }

                        throw;
                    }
                });

                await CreateNewTicketHistory(new CreateTicketHistoryDto
                {
                    TicketId = dto.TicketId,
                    Description = Messages.NewMessageSentByUser
                });

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(Messages.OperationFailedError);
            }
        }

        public async Task<Result> ClientSideCanUserAccessAtachment(string fileName)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1)
                return Result.Failure(Messages.OperationFailedError);

            if (await _context.TicketMessages.AnyAsync(n => n.AttachmentPath == fileName && n.Ticket.UserId == userId))
                return Result.Success();

            return Result.Failure(Messages.OperationFailedError);
        }

        public async Task<Result<AdminSideFilterTicketsDto>> AdminSideFilterTickets(AdminSideFilterTicketsDto filter)
        {
            if (filter is null)
                return Result.Failure<AdminSideFilterTicketsDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<Ticket>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Title.Contains(filter.Search));
            }

            if (filter.Status is not null)
            {
                conditions.Add(n => n.Status == filter.Status);
            }

            if (filter.Category is not null)
            {
                conditions.Add(n => n.Category == filter.Category);
            }

            if (filter.Priority is not null)
            {
                conditions.Add(n => n.Priority == filter.Priority);
            }

            IQueryable<Ticket> query = _context.Tickets;

            foreach (var item in conditions ?? new FilterConditions<Ticket>())
            {
                query = query.Where(item);
            }
            query = query.OrderByDescending(n => n.LastUpdatedDate);

            await filter.Paging(query.Select(n => new AdminSideTicketDto()
            {
                Id = n.Id,
                Status = n.Status,
                Category = n.Category,
                CreatedDate = n.CreatedDate,
                LastUpdatedDate = n.LastUpdatedDate,
                Priority = n.Priority,
                Title = n.Title
            }));

            return filter;
        }

        public async Task<Result<AdminSideTicketDetailsDto>> AdminSideGetTicketDetails(int ticketId)
        {
            var ticket = await _context.Tickets
                .AsNoTracking()
                .Where(n => n.Id == ticketId)
                .Select(n => new AdminSideTicketDetailsDto()
                {
                    Id = n.Id,
                    Category = n.Category,
                    CreatedDate = n.CreatedDate,
                    Status = n.Status,
                    Title = n.Title,
                    Messages = n.Messages
                    .OrderBy(m => m.Id)
                    .Select(m => new AdminSideTicketMessageDto()
                    {
                        Id = m.Id,
                        Text = m.Text,
                        CreatedDate = m.CreatedDate,
                        AttachmentPath = m.AttachmentPath,
                        IsAdminResponse = m.IsAdminResponse,
                    }).ToList()
                }).FirstOrDefaultAsync();
            if (ticket is null)
                return Result.Failure<AdminSideTicketDetailsDto>(Messages.OperationFailedError);

            return ticket;
        }

        public async Task<Result> AdminSideCloseTicket(int ticketId)
        {
            if (ticketId < 1)
                return Result.Failure(Messages.OperationFailedError);

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket is null)
                return Result.Failure(Messages.OperationFailedError);

            ticket.Status = TicketStatusEnum.Closed;
            ticket.LastUpdatedDate = DateTime.UtcNow;

            _context.Update(ticket);
            await _context.SaveChangesAsync();

            // Track admin closing the ticket
            await CreateNewTicketHistory(new CreateTicketHistoryDto
            {
                TicketId = ticketId,
                Description = Messages.TicketClosedByAdmin
            });

            return Result.Success();
        }

        public async Task<Result> AdminSideSendNewMessage(AdminSideSendTicketMessageDto dto)
        {
            if (dto is null)
                return Result.Failure(Messages.OperationFailedError);

            var userId = _httpContextAccessor.HttpContext?.User.GetUserId() ?? 0;
            if (userId < 1)
                return Result.Failure(Messages.OperationFailedError);

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == dto.TicketId);

            if (ticket is null)
                return Result.Failure(Messages.OperationFailedError);

            var ticketMessage = new TicketMessage()
            {
                TicketId = dto.TicketId,
                SenderId = userId,
                Text = dto.MessageText,
                IsAdminResponse = true,
                CreatedDate = DateTime.UtcNow,
            };

            string? fileName = null;
            if (dto.Attachment is not null)
            {
                if (!dto.Attachment.IsImage())
                    return Result.Failure(Messages.InvalidFileFormatError);

                if (dto.Attachment.Length > SiteTools.MaxTicketAttachmentLength.CalculateKB())
                    return Result.Failure(Messages.MaxLengthError);

                fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(dto.Attachment.FileName);
                ticketMessage.AttachmentPath = fileName;
            }

            ticket.Status = TicketStatusEnum.Answered;
            ticket.LastUpdatedDate = DateTime.UtcNow;

            var strategy = _context.Database.CreateExecutionStrategy();

            try
            {
                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        if (dto.Attachment is not null && fileName is not null)
                        {
                            dto.Attachment.UploadSensitiveFileToServer(fileName, SiteTools.TicketAttachmentFile);
                        }

                        _context.TicketMessages.Add(ticketMessage);
                        _context.Tickets.Update(ticket);

                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();

                        if (dto.Attachment is not null && fileName is not null)
                        {
                            fileName.RemoveSensitiveFileFromServer(SiteTools.TicketAttachmentFile);
                        }

                        throw;
                    }
                });

                await CreateNewTicketHistory(new CreateTicketHistoryDto
                {
                    TicketId = dto.TicketId,
                    Description = Messages.NewMessageSentByAdmin
                });

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(Messages.OperationFailedError);
            }
        }

        public async Task CreateNewTicketHistory(CreateTicketHistoryDto dto)
        {
            if (dto is null)
                return;

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            if (userId < 1)
                return;

            var history = new TicketHistory()
            {
                Description = dto.Description,
                TicketId = dto.TicketId,
                ActorId = userId
            };

            _context.TicketHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<Result<AdminSideFilterTicketHistoryDto>> AdminSideFilterTicketHistory(AdminSideFilterTicketHistoryDto filter)
        {
            if (filter is null)
                return Result.Failure<AdminSideFilterTicketHistoryDto>(Messages.OperationFailedError);

            var conditions = Filter.GenerateConditions<TicketHistory>();

            conditions.Add(n => n.TicketId == filter.TicketId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                conditions.Add(n => n.Description.Contains(filter.Search));
            }

            IQueryable<TicketHistory> query = _context.TicketHistories;

            foreach (var item in conditions)
            {
                query = query.Where(item);
            }

            query = query.OrderByDescending(n => n.Id);

            await filter.Paging(query.Select(n => new AdminSideTicketHistoryDto()
            {
                Id = n.Id,
                Description = n.Description,
                ActorId = n.ActorId,
                CreatedDate = n.CreatedDate
            }));

            return filter;
        }

        public async Task<Result> AdminSideUpdateTicketStatus(AdminSideUpdateTicketStatusDto dto)
        {
            if (dto is null)
                return Result.Failure(Messages.OperationFailedError);

            var ticket = await _context.Tickets.FindAsync(dto.Id);
            if (ticket is null)
                return Result.Failure(Messages.OperationFailedError);

            ticket.Status = dto.Status;
            ticket.LastUpdatedDate = DateTime.UtcNow;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            await CreateNewTicketHistory(new CreateTicketHistoryDto()
            {
                TicketId = ticket.Id,
                Description = Messages.TicketStatusUpdatedByAdmin
            });

            return Result.Success();
        }
    }
}
