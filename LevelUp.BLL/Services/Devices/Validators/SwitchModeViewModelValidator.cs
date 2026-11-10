using FluentValidation;
using LevelUp.BLL.Services.Devices.ViewModel;

namespace LevelUp.BLL.Services.Devices.Validators;

public class SwitchModeViewModelValidator : AbstractValidator<SwitchModeViewModel>
{
    public SwitchModeViewModelValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0).WithMessage("الغرفة مش موجودة");

        RuleFor(x => x.Mode)
            .IsInEnum().WithMessage("اختار سنجل أو ملتي");
    }
}
