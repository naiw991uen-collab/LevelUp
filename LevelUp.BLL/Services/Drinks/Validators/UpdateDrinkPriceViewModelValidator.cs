using FluentValidation;
using LevelUp.BLL.Services.Drinks.ViewModel;

namespace LevelUp.BLL.Services.Drinks.Validators;

public class UpdateDrinkPriceViewModelValidator : AbstractValidator<UpdateDrinkPriceViewModel>
{
    public UpdateDrinkPriceViewModelValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("المشروب مش موجود");

        RuleFor(x => x.Price)
            .NotNull().WithMessage("السعر لازم يكون أكبر من صفر")
            .InclusiveBetween(1, 100000).WithMessage("السعر لازم يكون أكبر من صفر");
    }
}
