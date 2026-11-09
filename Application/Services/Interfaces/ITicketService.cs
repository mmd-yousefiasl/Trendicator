using Application.Dtos.Tickets;
using Application.Dtos.Tickets.AdminSide;
using Application.Dtos.Tickets.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ITicketService
    {
        Task<Result<int>> ClientSideCreateNewTicket(ClientSideCreateNewTicketDto dto);

        Task<Result<ClientSideFilterTicketsDto>> ClientSideFilterTickets(ClientSideFilterTicketsDto filter);

        Task<Result<ClientSideTicketDetailsDto>> ClientSideGetTicketDetails(int ticketId);

        Task<Result> ClientSideCloseTicket(int ticketId);

        Task<Result> ClientSideSendNewMessage(ClientSideSendTicketMessageDto dto);

        Task<Result> ClientSideCanUserAccessAtachment(string fileName);

        Task<Result<AdminSideFilterTicketsDto>> AdminSideFilterTickets(AdminSideFilterTicketsDto filter);

        Task<Result<AdminSideTicketDetailsDto>> AdminSideGetTicketDetails(int ticketId);

        Task<Result> AdminSideCloseTicket(int ticketId);

        Task<Result> AdminSideSendNewMessage(AdminSideSendTicketMessageDto dto);

        Task CreateNewTicketHistory(CreateTicketHistoryDto dto);

        Task<Result<AdminSideFilterTicketHistoryDto>> AdminSideFilterTicketHistory(AdminSideFilterTicketHistoryDto filter);

        Task<Result> AdminSideUpdateTicketStatus(AdminSideUpdateTicketStatusDto dto);
    }
}
