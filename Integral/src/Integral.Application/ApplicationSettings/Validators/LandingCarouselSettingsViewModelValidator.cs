using FluentValidation;
using Integral.Application.ApplicationSettings.Data;
using System.Linq;

namespace Integral.Application.ApplicationSettings.Validators
{
    public class LandingCarouselSettingsViewModelValidator : AbstractValidator<LandingCarouselSettingsViewModel>
    {
        public LandingCarouselSettingsViewModelValidator()
        {

        }

        public class LandingCarouselSettingsViewModelItemValidator : AbstractValidator<LandingCarouselSettingsViewModel.Item>
        {
            public LandingCarouselSettingsViewModelItemValidator()
            {
                RuleFor(m => m.DisplayName).NotEmpty().WithMessage("Файл не выбран");
            }
        }
    }
}
