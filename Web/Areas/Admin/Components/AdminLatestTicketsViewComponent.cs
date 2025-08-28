using Application.Services.Interfaces;
using Application.ViewModels.Ticket.AdminSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class AdminLatestTicketsViewComponent : ViewComponent
    {
        private readonly ITicketService _ticketService;
        public AdminLatestTicketsViewComponent(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _ticketService.AdminSideFilterTickets(new AdminSideFilterTicketsViewModel());
            if (result.IsFailure) return View("AdminLatestTickets", new AdminSideFilterTicketsViewModel());

            return View("AdminLatestTickets", result.Value);
        }
    }
}
