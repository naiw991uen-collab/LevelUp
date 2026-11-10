using FluentValidation;
using LevelUp.BLL.Services.Sessions.ViewModel;

namespace LevelUp.BLL.Services.Sessions.Validators;

public class SessionFilterViewModelValidator : AbstractValidator<SessionFilterViewModel>
{
    public SessionFilterViewModelValidator()
    {
        RuleFor(x => x.Mode)
            .IsInEnum().WithMessage("اختار النمط من القايمة");

        RuleFor(x => x.RoomId)
            .GreaterThan(0).When(x => x.RoomId is not null).WithMessage("الغرفة مش موجودة");
    }
}
