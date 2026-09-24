using Application.Dtos.Ticket.ClientSide;
using Application.Services.Interfaces;
using Application.Dtos.Ticket.AdminSide;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Controllers
{
    public class TicketController : BaseUserPanelController
    {
        #region Fields
        private readonly ITicketService _ticketService;
        #endregion

        #region Ctor
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        #endregion

        [HttpGet("/user/my-tickets")]
        public async Task<IActionResult> MyTickets(ClientSideFilterTicketsViewModel filter)
        {
            var result = await _ticketService.ClientSideFilterTickets(filter);
            if (result.IsFailure) return View(new ClientSideFilterTicketsViewModel());

            return View(result.Value);
        }

        [HttpGet("/user/add-new-ticket")]
        public IActionResult CreateNewTicket()
        {
            return View();
        }

        [HttpPost("/user/add-new-ticket"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewTicket(ClientSideCreateNewTicketViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _ticketService.ClientSideCreateNewTicket(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(MyTickets));
        }

        [HttpGet("/user/ticket-details")]
        public async Task<IActionResult> TicketDetails(int id)
        {
            var result = await _ticketService.ClientSideGetTicketDetails(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(MyTickets));
            }

            return View(result.Value);
        }

        [HttpGet("/user/add-new-message")]
        public IActionResult CreateNewMessage(int id)
        {
            return View(new ClientSideCreateNewTicketMessageViewModel() { TicketId = id });
        }

        [HttpPost("/user/add-new-message"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewMessage(ClientSideCreateNewTicketMessageViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _ticketService.ClientSideCreateNewMessage(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(TicketDetails), new { id = model.TicketId });
        }

        [HttpPost("/user/close-ticket"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseTicket(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(MyTickets));
            }
            var result = await _ticketService.ClientSideClostTicket(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(MyTickets));
        }
    }
}
