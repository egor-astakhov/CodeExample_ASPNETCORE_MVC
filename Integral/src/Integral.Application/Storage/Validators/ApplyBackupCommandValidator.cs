using FluentValidation;
using Integral.Application.Storage.Commands;

namespace Integral.Application.Storage.Validators
{
    public class ApplyBackupCommandValidator : AbstractValidator<ApplyBackupCommand>
    {
        public ApplyBackupCommandValidator()
        {
            RuleFor(m => m.File)
                .NotEmpty().WithMessage("Поле не может быть пустым");
        }
    }
}
