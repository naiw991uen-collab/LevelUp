using FluentValidation;
using LevelUp.BLL.Services.Rooms.ViewModel;

namespace LevelUp.BLL.Services.Rooms.Validators;

public class UpdateRoomNameViewModelValidator : AbstractValidator<UpdateRoomNameViewModel>
{
    public UpdateRoomNameViewModelValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("الغرفة مش موجودة");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اكتب اسم الغرفة")
            .MaximumLength(50).WithMessage("اسم الغرفة طويل، أقصى حاجة 50 حرف");
    }
}
