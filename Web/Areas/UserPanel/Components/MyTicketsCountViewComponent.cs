using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Components
{
    public class MyTicketsCountViewComponent : ViewComponent
    {
        private readonly ITicketService _ticketService;
        public MyTicketsCountViewComponent(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _ticketService.MyTicketsCount();
            return View("MyTicketsCount", result);
        }
    }
}
