using FluentValidation;
using LevelUp.BLL.Services.Dashboard.ViewModel;

namespace LevelUp.BLL.Services.Dashboard.Validators;

public class UpdateRateViewModelValidator : AbstractValidator<UpdateRateViewModel>
{
    public UpdateRateViewModelValidator()
    {
        RuleFor(x => x.Mode)
            .IsInEnum().WithMessage("اختار سنجل أو ملتي");

        RuleFor(x => x.HourlyRate)
            .NotNull().WithMessage("اكتب سعر الساعة")
            .InclusiveBetween(1, 100000).WithMessage("السعر لازم يكون أكبر من صفر");
    }
}
