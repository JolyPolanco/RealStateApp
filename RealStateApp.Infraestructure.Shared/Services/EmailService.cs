using MailKit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using RealStateApp.Core.Application.Dtos.Email;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Infraestructure.Shared.Services
{
    public class EmailService : IEmailService
    {

        public  MailSettings _settings { get; set; }
        public EmailService(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }
        public async Task SendAsync(EmailRequestDto request)
        {
            request.ToRange?.Add(request.To ?? "");
            MimeMessage email = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(_settings.EmailFrom),
                Subject = request.Subject
            };
            foreach (var toItem in request.ToRange ?? [])
            {
                email.To.Add(MailboxAddress.Parse(toItem));

            }

            BodyBuilder builder = new BodyBuilder()
            {
                HtmlBody = request.BodyHtml
            };
            email.Body = builder.ToMessageBody();

            using MailKit.Net.Smtp.SmtpClient smtpClient = new();
            smtpClient.CheckCertificateRevocation = false;
            await smtpClient.ConnectAsync(_settings.SmptHost, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
            await smtpClient.SendAsync(email);
            await smtpClient.DisconnectAsync(true);
        }
    }
}
