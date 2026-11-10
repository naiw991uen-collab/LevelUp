using LevelUp.BLL.Common;
using LevelUp.BLL.Services.Account.Interfaces;
using LevelUp.BLL.Services.Account.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.PL.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Single", "Dashboard");

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _accountService.LoginAsync(model);

        if (!result.Success)
        {
            switch (result.Kind)
            {
                case ResultKind.NotFound:
                    // Wrong user name: clear it and show the error under the field
                    ModelState.Remove(nameof(model.UserName));
                    model.UserName = string.Empty;
                    ModelState.AddModelError(nameof(model.UserName), result.Error!);
                    break;

                case ResultKind.ValidationFailed:
                    // Wrong password: keep the user name, show the error under the password
                    ModelState.AddModelError(nameof(model.Password), result.Error!);
                    break;

                default:
                    ModelState.AddModelError(string.Empty, result.Error!);
                    break;
            }

            return View(model);
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Single", "Dashboard");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        return RedirectToAction(nameof(Login));
    }
}
