using FluentValidation;
using LevelUp.BLL.Services.Drinks.ViewModel;

namespace LevelUp.BLL.Services.Drinks.Validators;

public class CreateDrinkViewModelValidator : AbstractValidator<CreateDrinkViewModel>
{
    public CreateDrinkViewModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اكتب اسم المشروب")
            .MaximumLength(50).WithMessage("اسم المشروب طويل، أقصى حاجة 50 حرف");

        RuleFor(x => x.Price)
            .NotNull().WithMessage("السعر لازم يكون أكبر من صفر")
            .InclusiveBetween(1, 100000).WithMessage("السعر لازم يكون أكبر من صفر");
    }
}
