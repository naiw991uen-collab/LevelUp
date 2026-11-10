using FluentValidation;
using LevelUp.BLL.Services.Devices.ViewModel;

namespace LevelUp.BLL.Services.Devices.Validators;

public class RoomDrinkViewModelValidator : AbstractValidator<RoomDrinkViewModel>
{
    public RoomDrinkViewModelValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0).WithMessage("الغرفة مش موجودة");

        RuleFor(x => x.DrinkId)
            .GreaterThan(0).WithMessage("اختار مشروب");
    }
}
