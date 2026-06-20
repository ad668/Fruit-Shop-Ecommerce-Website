using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using OnlineFruitShop.Core.Interfaces;

namespace OnlineFruitShop.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public bool IsConfigured => _settings.IsConfigured;

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            if (!IsConfigured)
            {
                return;
            }

            using var message = new MailMessage();
            message.From = new MailAddress(_settings.FromEmail);
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = htmlBody;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = _settings.UseSsl,
                UseDefaultCredentials = false,
                Timeout = 10000
            };

            if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
            {
                client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
            }

            if (_settings.UseSsl && _settings.SmtpPort == 587)
            {
                client.TargetName = $"STARTTLS/{_settings.SmtpHost}";
            }

            await client.SendMailAsync(message);
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string htmlBody, byte[] attachmentData, string attachmentFileName)
        {
            if (!IsConfigured)
            {
                return;
            }

            using var message = new MailMessage();
            message.From = new MailAddress(_settings.FromEmail);
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = htmlBody;
            message.IsBodyHtml = true;

            // Add attachment
            using var attachmentStream = new MemoryStream(attachmentData);
            var attachment = new Attachment(attachmentStream, attachmentFileName, "application/pdf");
            message.Attachments.Add(attachment);

            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = _settings.UseSsl,
                UseDefaultCredentials = false,
                Timeout = 10000
            };

            if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
            {
                client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
            }

            if (_settings.UseSsl && _settings.SmtpPort == 587)
            {
                client.TargetName = $"STARTTLS/{_settings.SmtpHost}";
            }

            await client.SendMailAsync(message);
        }
    }
}

