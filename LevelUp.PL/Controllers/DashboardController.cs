using LevelUp.BLL.Services.Dashboard.Interfaces;
using LevelUp.BLL.Services.Dashboard.ViewModel;
using LevelUp.DAL.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.PL.Controllers;

[Authorize]
public class DashboardController : BaseController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> Single()
    {
        var model = await _dashboardService.GetAsync(GameMode.Single);
        return View("Index", model);
    }

    [HttpGet]
    public async Task<IActionResult> Multi()
    {
        var model = await _dashboardService.GetAsync(GameMode.Multi);
        return View("Index", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRate(UpdateRateViewModel model)
    {
        var mode = model.Mode == GameMode.Multi ? GameMode.Multi : GameMode.Single;

        if (!ModelState.IsValid)
        {
            var page = await _dashboardService.GetAsync(mode);
            page.NewRate = model;
            return View("Index", page);
        }

        SetMessage(await _dashboardService.UpdateRateAsync(model));
        return RedirectToAction(mode == GameMode.Multi ? nameof(Multi) : nameof(Single));
    }
}
