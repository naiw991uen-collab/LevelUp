using FluentValidation;
using LevelUp.BLL.Services.Account.ViewModel;

namespace LevelUp.BLL.Services.Account.Validators;

public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
{
    public LoginViewModelValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("اكتب اسم المستخدم");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("اكتب كلمة المرور");
    }
}
