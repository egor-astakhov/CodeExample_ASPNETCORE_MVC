using FluentValidation;

namespace Integral.Web.Models.Account
{
    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {
            RuleFor(m => m.UserName)
                .NotEmpty().WithMessage("Логин не может быть пустым");

            RuleFor(m => m.Password)
                .NotEmpty().WithMessage("Пароль не может быть пустым");
        }
    }
}
