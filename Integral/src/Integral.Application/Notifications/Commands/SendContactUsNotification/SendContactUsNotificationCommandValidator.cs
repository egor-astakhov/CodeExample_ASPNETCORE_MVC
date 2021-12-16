using FluentValidation;
namespace Integral.Application.Notifications.Commands.SendContactUsNotification
{
    public class SendContactUsNotificationCommandValidator : AbstractValidator<SendContactUsNotificationCommand>
    {
        public SendContactUsNotificationCommandValidator()
        {
            RuleFor(m => m.Name).NotEmpty().WithMessage("Имя не может быть пустым");

            RuleFor(m => m.Email)
                .NotEmpty().WithMessage("E-mail не может быть пустым")
                .EmailAddress().WithMessage("Некорректный e-mail");

            RuleFor(m => m.Subject)
                .NotEmpty().WithMessage("Тема не может быть пустой");

            RuleFor(m => m.Message)
                .NotEmpty().WithMessage("Сообщение не может быть пустым");
        }
    }
}
