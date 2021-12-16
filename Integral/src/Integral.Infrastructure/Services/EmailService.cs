using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Services;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Integral.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _client;

        private readonly MailAddress _sender;
        private readonly MailAddress _support;
        public EmailService(EmailServiceSettingsDTO settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings), "EmailService settings are not initialized.");
            }

            _client = new SmtpClient(settings.Host, settings.Port)
            {
                Credentials = new NetworkCredential(settings.Username, settings.Password),
                EnableSsl = true,
            };

            _sender = new MailAddress(settings.Username);
            _support = new MailAddress(settings.SupportEmail);
        }

        public async Task SendToSupportAsync(string subject, string message)
        {
            var mail = new MailMessage(_sender, _support)
            {
                Subject = subject,
                Body = message
            };

            await _client.SendMailAsync(mail);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
