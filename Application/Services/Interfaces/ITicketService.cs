using Application.ViewModels.Ticket.AdminSide;
using Application.ViewModels.Ticket.ClientSide;
using Domain.Shared;

namespace Application.Services.Interfaces
{
    public interface ITicketService
    {
        Task<Result<ClientSideFilterTicketsViewModel>> ClientSideFilterTickets(ClientSideFilterTicketsViewModel filter);

        Task<Result> ClientSideCreateNewTicket(ClientSideCreateNewTicketViewModel model);

        Task<Result<ClientSideTicketDetailsViewModel>> ClientSideGetTicketDetails(int id);

        Task<Result> ClientSideCreateNewMessage(ClientSideCreateNewTicketMessageViewModel model);

        Task<Result<AdminSideFilterTicketsViewModel>> AdminSideFilterTickets(AdminSideFilterTicketsViewModel filter);

        Task<Result<AdminSideTicketDetailsViewModel>> AdminSideGetTicketDetails(int id);

        Task<Result> AdminSideCreateNewMessage(AdminSideCreateNewTicketMessageViewModel model);

        Task<Result> SoftDeleteTicket(int id);

        Task<Result> AdminSideCloseTicket(int id);

        Task<Result> ClientSideClostTicket(int id);

        Task<int> MyTicketsCount();
    }
}
