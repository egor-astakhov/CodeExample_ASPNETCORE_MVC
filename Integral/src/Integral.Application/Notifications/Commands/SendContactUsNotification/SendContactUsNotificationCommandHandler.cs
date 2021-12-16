using Integral.Application.Common.Services;
using MediatR;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Integral.Application.Notifications.Commands.SendContactUsNotification
{
    public class SendContactUsNotificationCommandHandler : AsyncRequestHandler<SendContactUsNotificationCommand>
    {
        private readonly IEmailServiceFactory _emailServiceFactory;

        public SendContactUsNotificationCommandHandler(IEmailServiceFactory emailServiceFactory)
        {
            _emailServiceFactory = emailServiceFactory;
        }

        protected override async Task Handle(SendContactUsNotificationCommand request, CancellationToken cancellationToken)
        {
            using var emailService = await _emailServiceFactory.GetInstanceAsync();

            await emailService.SendToSupportAsync(GetSubject(request), GetMessage(request));
        }

        private static string GetSubject(SendContactUsNotificationCommand request)
        {
            return $"Сообщение с сайта: \"{request.Subject}\"";
        }

        private static string GetMessage(SendContactUsNotificationCommand request)
        {
            var message = new StringBuilder();

            message.AppendLine($"Имя: {request.Name}");
            message.AppendLine($"E-mail: {request.Email}");
            message.AppendLine($"Сообщение: {request.Message}");

            return message.ToString();
        }
    }
}
