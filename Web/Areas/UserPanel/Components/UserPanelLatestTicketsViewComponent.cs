using Application.Dtos.Tickets.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class UserPanelLatestTicketsViewComponent : ViewComponent
    {
        private readonly ITicketService _ticketService;
        public UserPanelLatestTicketsViewComponent(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _ticketService.ClientSideFilterTickets(new ClientSideFilterTicketsDto() { TakeEntity = 5 });

            if (result.IsFailure)
                return View("UserPanelLatestTickets", new ClientSideFilterTicketsDto());

            return View("UserPanelLatestTickets", result.Value);
        }
    }
}
