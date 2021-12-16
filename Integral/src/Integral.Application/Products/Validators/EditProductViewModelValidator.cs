using FluentValidation;
using Integral.Application.Products.Data;
using System.Data;

namespace Integral.Application.Products.Validators
{
    public class EditProductViewModelValidator : AbstractValidator<EditProductViewModel>
    {
        public EditProductViewModelValidator()
        {
            RuleFor(m => m.Name)
                .MaximumLength(50).WithMessage("Длина превышена")
                .NotEmpty().WithMessage("Название не заполнено");

            RuleFor(m => m.ShortDescription)
                .MaximumLength(100).WithMessage("Длина превышена")
                .NotEmpty().WithMessage("Краткое описание не заполнено");

            RuleFor(m => m.ImageName)
                .MaximumLength(50).WithMessage("Длина превышена")
                .NotEmpty().WithMessage("Изображение не выбрано");

            RuleFor(m => m.Description)
                .MaximumLength(2000).WithMessage("Длина превышена")
                .NotEmpty().WithMessage("Описание не заполнено");

            RuleFor(m => m.ApplicationArea)
                .MaximumLength(1000).WithMessage("Длина превышена")
                .NotEmpty().WithMessage("Область применения не заполнена");

            RuleFor(m => m.Features)
                .MaximumLength(1000).WithMessage("Длина превышена")
                .NotEmpty().WithMessage("Особенности не заполнены");

            RuleFor(m => m.SortingOrder)
                .NotNull().WithMessage("Не заполнено")
                .InclusiveBetween(0, 9999).WithMessage("Значение должно быть от 0 до 9999");
        }

        public class EditProductViewModelSpecificationValidator : AbstractValidator<EditProductViewModel.Specification>
        {
            public EditProductViewModelSpecificationValidator()
            {
                RuleFor(m => m.Name)
                    .MaximumLength(100).WithMessage("Длина превышена")
                    .NotEmpty().WithMessage("Параметр не заполнен");

                RuleFor(m => m.Value)
                    .MaximumLength(200).WithMessage("Длина превышена")
                    .NotEmpty().WithMessage("Значение не заполнено");
            }
        }

        public class EditProductViewModelAttachmentValidator : AbstractValidator<EditProductViewModel.Attachment>
        {
            public EditProductViewModelAttachmentValidator()
            {
                RuleFor(m => m.Name)
                    .MaximumLength(200).WithMessage("Длина превышена")
                    .NotEmpty().WithMessage("Название не заполнено");

                RuleFor(m => m.FileVersion)
                    .MaximumLength(20).WithMessage("Длина превышена")
                    .NotEmpty().WithMessage("Версия не заполнена");

                RuleFor(m => m.FileDate)
                    .NotEmpty().WithMessage("Дата не заполнена");

                RuleFor(m => m.FileName)
                    .MaximumLength(50).WithMessage("Длина превышена")
                    .NotEmpty().WithMessage("Файл не выбран");
            }
        }
    }
}
