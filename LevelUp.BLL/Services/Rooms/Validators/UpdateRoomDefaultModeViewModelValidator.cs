using FluentValidation;
using LevelUp.BLL.Services.Rooms.ViewModel;

namespace LevelUp.BLL.Services.Rooms.Validators;

public class UpdateRoomDefaultModeViewModelValidator : AbstractValidator<UpdateRoomDefaultModeViewModel>
{
    public UpdateRoomDefaultModeViewModelValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("الغرفة مش موجودة");

        RuleFor(x => x.DefaultMode)
            .IsInEnum().WithMessage("اختار النمط الافتراضي: سنجل أو ملتي");
    }
}
