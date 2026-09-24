using Application.Dtos.Ticket.AdminSide;
using Application.Services.Interfaces;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Web.Attributes;

namespace Web.Areas.Admin.Controllers
{
    [Permission("ManageTickets")]
    public class TicketController : BaseAdminController
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


        [Permission("TicketsList")]
        [HttpGet("/admin/tickets-list")]
        public async Task<IActionResult> TicketsList(AdminSideFilterTicketsViewModel filter)
        {
            var result = await _ticketService.AdminSideFilterTickets(filter);
            if (result.IsFailure) return View(new AdminSideFilterTicketsViewModel());

            return View(result.Value);
        }

        [Permission("DeleteTicket")]
        [HttpPost("/admin/remove-ticket"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(TicketsList));
            }
            var result = await _ticketService.SoftDeleteTicket(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(TicketsList));
        }

        [Permission("TicketDetails")]
        public async Task<IActionResult> TicketDetails(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(TicketsList));
            }
            var result = await _ticketService.AdminSideGetTicketDetails(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(TicketsList));
            }

            return View(result.Value);
        }

        [Permission("CreateNewMessage")]
        [HttpGet("/admin/add-new-message")]
        public IActionResult CreateNewMessage(int id)
        {
            return View(new AdminSideCreateNewTicketMessageViewModel() { TicketId = id });
        }

        [Permission("CreateNewMessage")]
        [HttpPost("/admin/add-new-message"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewMessage(AdminSideCreateNewTicketMessageViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _ticketService.AdminSideCreateNewMessage(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(TicketDetails), new { area = "Admin", id = model.TicketId });
        }

        [Permission("CloseTicket")]
        [HttpPost("/admin/close-ticket"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseTicket(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(TicketsList));
            }
            var result = await _ticketService.AdminSideCloseTicket(id);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }

            return RedirectToAction(nameof(TicketsList));
        }
    }
}
