using FluentValidation;
using Integral.Application.ApplicationSettings.Data;

namespace Integral.Application.ApplicationSettings.Validators
{
    public class CommonSettingsViewModelValidator : AbstractValidator<CommonSettingsViewModel>
    {
        public CommonSettingsViewModelValidator()
        {
            //RuleFor(m => m.Address)
            //    .NotEmpty().WithMessage("Адрес не может быть пустым");

            //RuleFor(m => m.Email)
            //    .NotEmpty().WithMessage("Почта не может быть пустой")
            //    .EmailAddress().WithMessage("Почта должна быть корректным E-mail адресом");

            //RuleFor(m => m.PhoneNumber)
            //    .NotEmpty().WithMessage("Номер телефона не может быть пустым");
        }
    }
}
