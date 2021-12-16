using FluentValidation;
using Integral.Application.ApplicationSettings.Data;

namespace Integral.Application.ApplicationSettings.Validators
{
    public class EmailServiceSettingsViewModelValidator : AbstractValidator<EmailServiceSettingsViewModel>
    {
        public EmailServiceSettingsViewModelValidator()
        {
            RuleFor(m => m.Host)
                .NotEmpty().WithMessage("Поле не может быть пустым");

            RuleFor(m => m.Username)
                .NotEmpty().WithMessage("Поле не может быть пустым")
                .EmailAddress().WithMessage("Некорректный e-mail адрес");

            RuleFor(m => m.Password)
                .NotEmpty().WithMessage("Поле не может быть пустым");

            RuleFor(m => m.SupportEmail)
                .NotEmpty().WithMessage("Поле не может быть пустым")
                .EmailAddress().WithMessage("Некорректный e-mail адрес");
        }
    }
}
