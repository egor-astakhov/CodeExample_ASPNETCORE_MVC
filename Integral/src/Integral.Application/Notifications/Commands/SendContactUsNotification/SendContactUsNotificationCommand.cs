using MediatR;

namespace Integral.Application.Notifications.Commands.SendContactUsNotification
{
    public class SendContactUsNotificationCommand : IRequest
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }
    }
}
