using Application.Dtos.Tickets.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class ClientSideTicketDetailsViewComponent : ViewComponent
    {
        #region Fileds
        private readonly ITicketService _ticketService;
        #endregion

        #region Ctor
        public ClientSideTicketDetailsViewComponent(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        #endregion

        public async Task<IViewComponentResult> InvokeAsync(int ticketId)
        {
            if (ticketId < 1)
                return View("ClientSideTicketDetails", new ClientSideTicketDetailsDto());

            var result = await _ticketService.ClientSideGetTicketDetails(ticketId);
            if (result.IsFailure)
                return View("ClientSideTicketDetails", new ClientSideTicketDetailsDto());

            return View("ClientSideTicketDetails", result.Value);
        }
    }
}
