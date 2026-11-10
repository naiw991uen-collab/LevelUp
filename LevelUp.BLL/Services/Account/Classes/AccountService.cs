using LevelUp.BLL.Common;
using LevelUp.BLL.Services.Account.Interfaces;
using LevelUp.BLL.Services.Account.ViewModel;
using LevelUp.DAL.Applications;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LevelUp.BLL.Services.Account.Classes;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountService> _logger;

    public AccountService(UserManager<ApplicationUser> userManager,
                          SignInManager<ApplicationUser> signInManager,
                          ILogger<AccountService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    // Kind tells the controller which field the error belongs to:
    // NotFound = UserName, ValidationFailed = Password, Forbidden = locked out (whole form)
    public async Task<Result> LoginAsync(LoginViewModel model)
    {
        try
        {
            var userName = model.UserName.Trim();

            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
            {
                _logger.LogWarning("Failed login: user {UserName} not found", userName);
                return Result.NotFound("اسم المستخدم غلط");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password,
                model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserName} logged in", userName);
                return Result.OK();
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {UserName} is locked out", userName);
                return Result.Fail("الحساب اتقفل مؤقتاً بسبب محاولات غلط كتير، جرّب تاني بعد دقيقتين", ResultKind.Forbidden);
            }

            _logger.LogWarning("Failed login: wrong password for {UserName}", userName);
            return Result.Validation("كلمة المرور غلط");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for {UserName}", model.UserName);
            return Result.Fail("حصلت مشكلة أثناء تسجيل الدخول، جرّب تاني");
        }
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
    }
}
