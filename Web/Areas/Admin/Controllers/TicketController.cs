using Application.Dtos.Tickets.AdminSide;
using Application.Services.Interfaces;
using Application.Tools;
using Domain.Shared;
using Domain.Statics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
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
        public async Task<IActionResult> TicketsList(AdminSideFilterTicketsDto filter)
        {
            var result = await _ticketService.AdminSideFilterTickets(filter);
            if (result.IsFailure)
                return View(new AdminSideFilterTicketsDto());

            return View(result.Value);
        }

        [Permission("TicketDetails")]
        [HttpGet("/admin/ticket-details")]
        public IActionResult TicketDetails(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(TicketsList));
            }

            return View(new AdminSideSendTicketMessageDto()
            {
                TicketId = id
            });
        }

        [Permission("TicketDetails")]
        [HttpPost("/admin/ticket-details"), ValidateAntiForgeryToken]
        public async Task<IActionResult> TicketDetails(AdminSideSendTicketMessageDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _ticketService.AdminSideSendNewMessage(dto);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(dto);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(TicketDetails), new { id = dto.TicketId });
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

            return RedirectToAction(nameof(TicketDetails), new { id = id });
        }

        [Permission("TicketDetails")]
        [HttpGet("/admin/ticket-attachment/{fileName}")]
        public async Task<IActionResult> GetTicketAttachment(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return NotFound();

            string savePath = fileName.GetSensitiveFilePath(SiteTools.TicketAttachmentFile);

            if (!System.IO.File.Exists(savePath))
                return NotFound();

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(savePath, out string? contentType))
            {
                contentType = "application/octet-stream";
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(savePath);
            return File(fileBytes, contentType);
        }

        [Permission("TicketHistory")]
        [HttpGet("/admin/ticket-history")]
        public async Task<IActionResult> TicketHistory(AdminSideFilterTicketHistoryDto filter)
        {
            var result = await _ticketService.AdminSideFilterTicketHistory(filter);
            if (result.IsFailure)
                return View(new AdminSideFilterTicketHistoryDto()
                {
                    TicketId = filter.TicketId
                });

            return View(result.Value);
        }

        [Permission("UpdateTicketStatus")]
        [HttpPost("/admin/edit-ticket-status"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTicketStatus(AdminSideUpdateTicketStatusDto dto)
        {
            var result = await _ticketService.AdminSideUpdateTicketStatus(dto);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
            }
            else
            {
                TempData[TemporarySuccessMessage] = result.Message;
            }
            return RedirectToAction(nameof(TicketDetails), new { id = dto.Id });
        }
    }
}
