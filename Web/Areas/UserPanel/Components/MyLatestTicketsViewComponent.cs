using Application.Services.Interfaces;
using Application.ViewModels.Ticket.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class MyLatestTicketsViewComponent : ViewComponent
    {
        private readonly ITicketService _ticketService;
        public MyLatestTicketsViewComponent(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _ticketService.ClientSideFilterTickets(new ClientSideFilterTicketsViewModel());
            if (result.IsFailure) return View("MyLatestTickets", new ClientSideFilterTicketsViewModel());

            return View("MyLatestTickets", result.Value);
        }
    }
}
