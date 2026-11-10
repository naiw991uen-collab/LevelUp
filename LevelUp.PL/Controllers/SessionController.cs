using LevelUp.BLL.Services.Sessions.Interfaces;
using LevelUp.BLL.Services.Sessions.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.PL.Controllers;

[Authorize]
public class SessionController : BaseController
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(SessionFilterViewModel filter)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = FirstError();
            filter = new SessionFilterViewModel();
        }

        var model = await _sessionService.GetIndexAsync(filter);
        return View(model);
    }
}
