using FluentValidation;
using LevelUp.BLL.Services.Rooms.ViewModel;

namespace LevelUp.BLL.Services.Rooms.Validators;

public class CreateRoomViewModelValidator : AbstractValidator<CreateRoomViewModel>
{
    public CreateRoomViewModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اكتب اسم الغرفة الأول")
            .MaximumLength(50).WithMessage("اسم الغرفة طويل، أقصى حاجة 50 حرف");

        RuleFor(x => x.DefaultMode)
            .IsInEnum().WithMessage("اختار النمط الافتراضي: سنجل أو ملتي");
    }
}
