using Application.Dtos.Tickets.AdminSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Components
{
    public class AdminSideTicketDetailsViewComponent : ViewComponent
    {
        #region Fields
        private readonly ITicketService _ticketService;
        #endregion

        #region Ctor
        public AdminSideTicketDetailsViewComponent(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        #endregion

        public async Task<IViewComponentResult> InvokeAsync(int ticketId)
        {
            if (ticketId < 1)
                return View("AdminSideTicketDetails", new AdminSideTicketDetailsDto());

            var result = await _ticketService.AdminSideGetTicketDetails(ticketId);
            if (result.IsFailure)
            {
                return View("AdminSideTicketDetails", new AdminSideTicketDetailsDto());
            }

            return View("AdminSideTicketDetails", result.Value);
        }
    }
}
