using Application.Services.Interfaces;
using Application.ViewModels.SendEmail;
using Domain.Shared;
using MailKit.Net.Smtp;
using MimeKit;

namespace Application.Services.Implementaions
{
    public class SendEmailService : ISendEmailService
    {
        #region Fields
        private readonly IEmailDataService _emailDataService;
        private readonly IViewRenderService _viewRenderService;
        #endregion

        #region Ctor
        public SendEmailService(IEmailDataService emailDataService,
            IViewRenderService viewRenderService)
        {
            _emailDataService = emailDataService;
            _viewRenderService = viewRenderService;
        }
        #endregion

        public async Task<Result> SendBuckEmail(SendBuckEmailViewModel model)
        {
            var emailSettingResult = await _emailDataService.GetEmailData();
            if (emailSettingResult.IsFailure) return Result.Failure(emailSettingResult.Message);

            var emailSetting = emailSettingResult.Value;

            if (emailSetting != null)
            {
                var fromEmail = emailSetting.From;
                string password = emailSetting.Password;

                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(emailSetting.EmailDisplayName, fromEmail));

                    foreach (var recipient in model.Recipients ?? new List<string>())
                    {
                        message.To.Add(MailboxAddress.Parse(recipient));
                    }

                    message.Subject = model.Subject;

                    var htmlBody = await _viewRenderService.RenderToStringAsync("EmailTemplates/NewsLetters", model.Body);
                    message.Body = new TextPart("html") { Text = htmlBody };

                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync(emailSetting.Smtp, emailSetting.Port, true);
                        await client.AuthenticateAsync(fromEmail, password);
                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);
                    }

                    return Result.Success(Messages.SuccessfullyDone);
                }
                catch (Exception e)
                {
                    return Result.Failure(e.Message);
                }
            }

            return Result.Failure(Messages.OperationFailedError);
        }
    }
}
