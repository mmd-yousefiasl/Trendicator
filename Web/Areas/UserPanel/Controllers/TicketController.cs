using Application.Dtos.Tickets.ClientSide;
using Application.Services.Interfaces;
using Application.Tools;
using Domain.Shared;
using Domain.Statics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

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

        [HttpGet("/user/tickets")]
        public async Task<IActionResult> TicketsList(ClientSideFilterTicketsDto filter)
        {
            var result = await _ticketService.ClientSideFilterTickets(filter);
            if (result.IsFailure)
                return View(new ClientSideFilterTicketsDto());

            return View(result.Value);
        }

        [HttpGet("/user/ticket-details")]
        public IActionResult TicketDetails(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(TicketsList));
            }

            return View(new ClientSideSendTicketMessageDto()
            {
                TicketId = id
            });
        }

        [HttpPost("/user/ticket-details"), ValidateAntiForgeryToken]
        public async Task<IActionResult> TicketDetails(ClientSideSendTicketMessageDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _ticketService.ClientSideSendNewMessage(dto);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(dto);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(TicketDetails), new { id = dto.TicketId });
        }

        [HttpPost("/user/close-ticket/{id}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseTicket(int id)
        {
            if (id < 1)
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(TicketsList));
            }

            var result = await _ticketService.ClientSideCloseTicket(id);
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

        [HttpGet("/user/add-ticket")]
        public IActionResult CreateNewTicket()
        {
            return View();
        }

        [HttpPost("/user/add-ticket"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewTicket(ClientSideCreateNewTicketDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _ticketService.ClientSideCreateNewTicket(dto);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(dto);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(TicketDetails), new { id = result.Value });
        }

        [HttpGet("/user/ticket-attachment/{fileName}")]
        public async Task<IActionResult> GetTicketAttachment(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return NotFound();

            var result = await _ticketService.ClientSideCanUserAccessAtachment(fileName);
            if (result.IsFailure)
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
    }
}
